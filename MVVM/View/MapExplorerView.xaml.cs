using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI.Wpf;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using PERSONAL_PROJECT_2.MVVM.Model;
using PERSONAL_PROJECT_2.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PERSONAL_PROJECT_2.MVVM.View
{
    public partial class MapExplorerView : UserControl
    {
        private Mapsui.Map map;
        private MemoryLayer photoLayer;

        private MapExplorerViewModel viewModel;

        private MPoint? popupMapPosition;

        private List<PhotoGroup> photoGroups = new();
        private const double GroupDistanceMeters = 50;
        private PhotoGroup currentPhotoGroup;
        private int currentPhotoIndex = 0;

        private readonly HttpClient httpClient = new HttpClient();
        private readonly Dictionary<string, string> locationCache = new();

        public MapExplorerView()
        {
            InitializeComponent();

            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("PERSONAL_PROJECT_2.1/1.0");

            map = new Mapsui.Map();
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            photoLayer = new MemoryLayer("Photos")
            {
                Features = new List<IFeature>()
            };

            map.Layers.Add(photoLayer);
            mapControl.Map = map;

            mapControl.MapTapped += MapControl_MapTapped;
            mapControl.Map.Navigator.ViewportChanged += Navigator_ViewportChanged;

            DataContextChanged += MapExplorerView_DataContextChanged;
        }

        private void MapExplorerView_DataContextChanged(
            object sender,
            DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is MapExplorerViewModel oldViewModel)
            {
                oldViewModel.PhotoReceived -= AddPhotoToMap;
            }

            if (e.NewValue is MapExplorerViewModel newViewModel)
            {
                viewModel = newViewModel;
                viewModel.PhotoReceived += AddPhotoToMap;

                foreach (var photo in viewModel.Photos)
                {
                    AddPhotoToMap(photo);
                }

                System.Diagnostics.Debug.WriteLine(
                    "MapExplorerView connected to MapExplorerViewModel.");
                System.Diagnostics.Debug.WriteLine(
                    $"Existing photos: {viewModel.Photos.Count}");
            }
        }

        private void AddPhotoToMap(PhotoInfo photo)
        {
            if (!photo.Latitude.HasValue ||
                !photo.Longitude.HasValue)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Skipping {photo.Filename} - no GPS coordinates.");

                return;
            }

            PhotoGroup nearbyGroup = null;

            foreach (var group in photoGroups)
            {
                var firstPhoto = group.FirstPhoto;
                double distance = CalculateDistanceMeters(
                    photo.Latitude.Value,
                    photo.Longitude.Value,
                    firstPhoto.Latitude.Value,
                    firstPhoto.Longitude.Value);

                if (distance <= GroupDistanceMeters)
                {
                    nearbyGroup = group;
                    break;
                }
            }

            if (nearbyGroup != null)
            {
                nearbyGroup.Photos.Add(photo);
                System.Diagnostics.Debug.WriteLine(
                    $"Added {photo.Filename} to existing group.");
                System.Diagnostics.Debug.WriteLine(
                    $"Group now contains {nearbyGroup.Photos.Count} photos.");

                return;
            }

            var newGroup = new PhotoGroup();
            newGroup.Photos.Add(photo);
            photoGroups.Add(newGroup);

            System.Diagnostics.Debug.WriteLine(
                $"Created new photo group for {photo.Filename}");

            CreateGroupMarker(newGroup);
        }

        private string CreateCircularThumbnail(string photoPath, string filename)
        {
            string localAppDataPath =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData);
            string thumbnailDirectory = Path.Combine(
                localAppDataPath,
                "PERSONAL_PROJECT",
                "thumbnails");
            System.IO.Directory.CreateDirectory(thumbnailDirectory);
            string thumbnailPath = Path.Combine(
                thumbnailDirectory,
                Path.GetFileNameWithoutExtension(filename) + ".png");

            if (File.Exists(thumbnailPath))
                return thumbnailPath;

            const int size = 80;
            const int borderThickness = 4;
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri(photoPath);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.DecodePixelWidth = size;
            bitmap.DecodePixelHeight = size;
            bitmap.EndInit();

            var drawingVisual = new DrawingVisual();

            using (DrawingContext drawingContext =
                   drawingVisual.RenderOpen())
            {
                drawingContext.PushClip(
                    new EllipseGeometry(
                        new Point(size / 2.0, size / 2.0),
                        size / 2.0 - borderThickness,
                        size / 2.0 - borderThickness));

                drawingContext.DrawImage(
                    bitmap,
                    new Rect(
                        borderThickness,
                        borderThickness,
                        size - borderThickness * 2,
                        size - borderThickness * 2));

                drawingContext.Pop();

                var borderPen = new System.Windows.Media.Pen(
                    System.Windows.Media.Brushes.White,
                    borderThickness);

                drawingContext.DrawEllipse(
                    null,
                    borderPen,
                    new Point(size / 2.0, size / 2.0),
                    size / 2.0 - borderThickness / 2.0,
                    size / 2.0 - borderThickness / 2.0);
            }

            var renderBitmap = new RenderTargetBitmap(
                size,
                size,
                96,
                96,
                PixelFormats.Pbgra32);

            renderBitmap.Render(drawingVisual);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(
                BitmapFrame.Create(renderBitmap));

            using (var fileStream = new FileStream(
                thumbnailPath,
                FileMode.Create))
            {
                encoder.Save(fileStream);
            }
            return thumbnailPath;
        }

        private void MapControl_MapTapped(object? sender, Mapsui.MapEventArgs e)
        {
            var mapInfo = e.GetMapInfo(
                new[] { photoLayer });

            if (mapInfo == null ||
                mapInfo.Feature == null)
            {
                HidePhotoPopup();
                return;
            }
            if (mapInfo.Feature["PhotoGroup"]
                is not PhotoGroup group)
            {
                HidePhotoPopup();
                return;
            }

            ShowPhotoGroupPopup(
                group,
                mapInfo.Feature);
        }

        private void ShowPhotoGroupPopup(PhotoGroup group, IFeature feature)
        {
            currentPhotoGroup = group;
            currentPhotoIndex = 0;

            var point = ((PointFeature)feature).Point;

            popupMapPosition = point;

            photoPopup.Visibility = Visibility.Visible;

            UpdatePopupPosition();

            UpdatePopupImage();
        }

        private void HidePhotoPopup()
        {
            photoPopup.Visibility = Visibility.Collapsed;
            popupImage.Source = null;
            popupMapPosition = null;
            currentPhotoGroup = null;
            currentPhotoIndex = 0;
        }

        private void UpdatePopupPosition()
        {
            if (popupMapPosition == null)
                return;
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(UpdatePopupPosition));
                return;
            }

            var screenPosition =
                mapControl.Map.Navigator.Viewport
                .WorldToScreen(popupMapPosition);

            photoPopup.Margin = new Thickness(
                screenPosition.X - photoPopup.Width / 2,
                screenPosition.Y - photoPopup.Height - 20,
                0,
                0);
        }

        private void UpdatePopupImage()
        {
            if (currentPhotoGroup == null ||
                currentPhotoGroup.Photos.Count == 0)
            {
                return;
            }

            var photo = currentPhotoGroup.Photos[currentPhotoIndex];

            popupLocation.Text = photo.LocationName;

            string photoPath = Path.Combine(
                PhotoStorage.PhotosFolder,
                photo.Filename);

            if (!File.Exists(photoPath))
                return;

            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri(photoPath);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            popupImage.Source = LoadPhotoWithOrientation(photoPath);

            RenderOptions.SetBitmapScalingMode(
                popupImage,
                BitmapScalingMode.HighQuality);

            photoCounter.Text =
                $"{currentPhotoIndex + 1} / " +
                $"{currentPhotoGroup.Photos.Count}";

            previousPhotoButton.Visibility =
                currentPhotoGroup.Photos.Count > 1
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            nextPhotoButton.Visibility =
                currentPhotoGroup.Photos.Count > 1
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void Navigator_ViewportChanged(object? sender, ViewportChangedEventArgs e)
        {
            if (photoPopup.Visibility == Visibility.Visible)
            {
                UpdatePopupPosition();
            }
        }

        private static double CalculateDistanceMeters(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            const double earthRadius = 6371000;

            double lat1 = latitude1 * Math.PI / 180;
            double lat2 = latitude2 * Math.PI / 180;
            double deltaLat =
                (latitude2 - latitude1) * Math.PI / 180;
            double deltaLon =
                (longitude2 - longitude1) * Math.PI / 180;
            double a =
                Math.Sin(deltaLat / 2) *
                Math.Sin(deltaLat / 2)
                +
                Math.Cos(lat1) *
                Math.Cos(lat2) *
                Math.Sin(deltaLon / 2) *
                Math.Sin(deltaLon / 2);
            double c =
                2 * Math.Atan2(
                    Math.Sqrt(a),
                    Math.Sqrt(1 - a));

            return earthRadius * c;
        }

        private void CreateGroupMarker(PhotoGroup group)
        {
            var photo = group.FirstPhoto;
            string localAppDataPath =
                Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);
            string photoPath = Path.Combine(
                localAppDataPath,
                "PERSONAL_PROJECT",
                "photos",
                photo.Filename);

            if (!File.Exists(photoPath))
                return;

            var point = SphericalMercator.FromLonLat(
                photo.Longitude.Value,
                photo.Latitude.Value);
            var feature = new PointFeature(
                point.x,
                point.y);

            feature["PhotoGroup"] = group;

            string thumbnailPath =
                CreateCircularThumbnail(
                    photoPath,
                    photo.Filename);

            var imageStyle = new ImageStyle
            {
                Image = $"file://{thumbnailPath}",
                SymbolScale = 0.8
            };

            feature.Styles.Add(imageStyle);

            var features =
                photoLayer.Features.ToList();

            features.Add(feature);

            photoLayer.Features = features;
            photoLayer.DataHasChanged();
            System.Diagnostics.Debug.WriteLine($"Created marker for {photo.Filename}");
        }

        private void PreviousPhoto_Click(object sender, RoutedEventArgs e)
        {
            if (currentPhotoGroup == null)
                return;

            currentPhotoIndex--;

            if (currentPhotoIndex < 0)
            {
                currentPhotoIndex = currentPhotoGroup.Photos.Count - 1;
            }

            UpdatePopupImage();
        }
        private void NextPhoto_Click(object sender, RoutedEventArgs e)
        {
            if (currentPhotoGroup == null)
                return;

            currentPhotoIndex++;

            if (currentPhotoIndex >= currentPhotoGroup.Photos.Count)
            {
                currentPhotoIndex = 0;
            }

            UpdatePopupImage();
        }

        private async Task<string> GetLocationName(double latitude, double longitude)
        {
            string cacheKey = $"{latitude:F5},{longitude:F5}";

            if (locationCache.TryGetValue(
                cacheKey,
                out string cachedLocation))
            {
                return cachedLocation;
            }

            try
            {
                string url =
                    "https://nominatim.openstreetmap.org/reverse" +
                    $"?format=jsonv2" +
                    $"&lat={latitude}" +
                    $"&lon={longitude}" +
                    $"&zoom=12" +
                    $"&addressdetails=1" +
                    $"&layer=address";

                using HttpResponseMessage response =
                    await httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                using JsonDocument document = JsonDocument.Parse(json);

                JsonElement root = document.RootElement;

                if (!root.TryGetProperty(
                    "address",
                    out JsonElement address))
                {
                    return "Unknown location";
                }

                string location = GetBestLocationName(address);

                locationCache[cacheKey] = location;

                return location;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Reverse geocoding failed: {ex.Message}");

                return "Unknown location";
            }
        }

        private static string GetBestLocationName(JsonElement address)
        {
            if (address.TryGetProperty(
                "town",
                out JsonElement town))
            {
                return town.GetString();
            }

            if (address.TryGetProperty(
                "city",
                out JsonElement city))
            {
                return city.GetString();
            }

            if (address.TryGetProperty(
                "village",
                out JsonElement village))
            {
                return village.GetString();
            }

            if (address.TryGetProperty(
                "municipality",
                out JsonElement municipality))
            {
                return municipality.GetString();
            }

            if (address.TryGetProperty(
                "suburb",
                out JsonElement suburb))
            {
                return suburb.GetString();
            }

            if (address.TryGetProperty(
                "county",
                out JsonElement county))
            {
                return county.GetString();
            }

            return "Unknown location";
        }

        private BitmapSource LoadPhotoWithOrientation(string photoPath)
        {
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri(photoPath);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            bitmap.EndInit();

            bitmap.Freeze();

            var metadata = ImageMetadataReader.ReadMetadata(photoPath);

            var exif = metadata
                .OfType<ExifIfd0Directory>()
                .FirstOrDefault();

            if (exif == null)
                return bitmap;

            if (!exif.TryGetInt32(
                ExifDirectoryBase.TagOrientation,
                out int orientation))
            {
                return bitmap;
            }

            switch (orientation)
            {
                // Normal
                case 1:
                    return bitmap;

                // Flip horizontal
                case 2:
                    return new TransformedBitmap(
                        bitmap,
                        new ScaleTransform(-1, 1));

                // Rotate 180
                case 3:
                    return new TransformedBitmap(
                        bitmap,
                        new RotateTransform(180));

                // Flip vertical
                case 4:
                    return new TransformedBitmap(
                        bitmap,
                        new ScaleTransform(1, -1));

                // Flip horizontal + rotate 270
                case 5:
                    return new TransformedBitmap(
                        bitmap,
                        new TransformGroup
                        {
                            Children =
                            {
                        new ScaleTransform(-1, 1),
                        new RotateTransform(270)
                            }
                        });

                // Rotate 90
                case 6:
                    return new TransformedBitmap(
                        bitmap,
                        new RotateTransform(90));

                // Flip horizontal + rotate 90
                case 7:
                    return new TransformedBitmap(
                        bitmap,
                        new TransformGroup
                        {
                            Children =
                            {
                        new ScaleTransform(-1, 1),
                        new RotateTransform(90)
                            }
                        });

                // Rotate 270
                case 8:
                    return new TransformedBitmap(
                        bitmap,
                        new RotateTransform(270));

                default:
                    return bitmap;
            }
        }
    }
}
