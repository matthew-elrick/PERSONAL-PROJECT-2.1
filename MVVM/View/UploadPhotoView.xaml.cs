using MetadataExtractor;
using MetadataExtractor.Formats.Apple;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using Microsoft.Win32;
using PERSONAL_PROJECT_2.CustomPhotoControls;
using PERSONAL_PROJECT_2.MVVM.Model;
using PERSONAL_PROJECT_2.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Diagnostics;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Text.Json;

namespace PERSONAL_PROJECT_2.MVVM.View
{
    /// <summary>
    /// Interaction logic for UploadPhotoView.xaml
    /// </summary>
    public partial class UploadPhotoView : UserControl
    {
        private readonly HttpClient httpClient = new HttpClient();

        public UploadPhotoView()
        {
            InitializeComponent();

            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("PERSONAL_PROJECT_2.1/1.0");
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Process_Photo();
        }

        private async void Rectangle_Drop(object sender, DragEventArgs e)
        {
            await Process_Photo();
        }

        public async Task Process_Photo()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Multiselect = true };
            openFileDialog.Filter = "Image Files (*.gif,*.jpg,*.jpeg,*.bmp,*.png)|*.gif;*.jpg;*.jpeg;*.bmp;*.png";
            openFileDialog.FilterIndex = 1;
            bool? response = openFileDialog.ShowDialog();
            if (response == true)
            {
                string[] files = openFileDialog.FileNames;

                for (int i = 0; i < files.Length; i++)
                {
                    string filename = System.IO.Path.GetFileName(files[i]);
                    double? latitude = null;
                    double? longitude = null;
                    FileInfo fileInfo = new FileInfo(files[i]);
                    UploadingFilesList.Items.Add(new FileDeatil()
                    {
                        FileName = filename,
                        FileSize = string.Format("{0} {1}", (fileInfo.Length / 1.049e+6).ToString("0.0"), "Mb"),
                        UploadProgress = 100

                    });

                    string dir = PhotoStorage.PhotosFolder;

                    if (!System.IO.Directory.Exists(dir))
                    {
                        System.IO.Directory.CreateDirectory(dir);
                    }

                    string sourcePath = files[i];
                    string filenameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(filename);
                    string extension = System.IO.Path.GetExtension(filename);
                    string targetPath = System.IO.Path.Combine(dir, filename);

                    int counter = 1;

                    while (File.Exists(targetPath))
                    {
                        string newFilename = $"{filenameWithoutExtension}_{counter}{extension}";
                        targetPath = System.IO.Path.Combine(dir, newFilename);
                        counter++;
                    }
                    File.Copy(sourcePath, targetPath);

                    string storedFilename = System.IO.Path.GetFileName(targetPath);

                    try
                    {
                        var gps = ImageMetadataReader.ReadMetadata(files[i]).OfType<GpsDirectory>().FirstOrDefault();

                        if (gps == null)
                        {
                            System.Diagnostics.Debug.WriteLine("No GPS found.");
                        }
                        else if (gps.GetGeoLocation() is GeoLocation location)
                        {

                            latitude = location.Latitude;
                            longitude = location.Longitude;

                            System.Diagnostics.Debug.WriteLine(files[i]);
                            System.Diagnostics.Debug.WriteLine(
                                $"Latitude: {location.Latitude}");
                            System.Diagnostics.Debug.WriteLine(
                                $"Longitude: {location.Longitude}");
                            System.Diagnostics.Debug.WriteLine("-----------------------------------");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("GPS found, but no coordinates.");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }

                    double photoLatitude = latitude ?? 0;
                    double photoLongitude = longitude ?? 0;

                    string locationName = "Unknown location";

                    if (latitude.HasValue &&
                        longitude.HasValue)
                    {
                        locationName = await GetLocationName(
                            photoLatitude,
                            photoLongitude);
                    }

                    PhotoInfo photoInfo = new PhotoInfo
                    {
                        Filename = storedFilename,
                        Latitude = latitude ?? 0,
                        Longitude = longitude ?? 0
                    };

                    var viewModel = DataContext as UploadPhotoViewModel;

                    if (latitude.HasValue && longitude.HasValue)
                    {
                        viewModel?.NotifyPhotoUploaded(photoInfo);
                    }
                    else
                    {
                        viewModel?.NotifyPhotoNeedsLocation(photoInfo);
                    }
                }
            }

        }

        private async Task<string> GetLocationName(double latitude, double longitude)
        {
            try
            {
                string url =
                    "https://nominatim.openstreetmap.org/reverse" +
                    $"?format=jsonv2" +
                    $"&lat={latitude}" +
                    $"&lon={longitude}" +
                    $"&zoom=12" +
                    $"&addressdetails=1";

                using HttpResponseMessage response =
                    await httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                string json =
                    await response.Content.ReadAsStringAsync();

                using JsonDocument document =
                    JsonDocument.Parse(json);

                JsonElement address =
                    document.RootElement.GetProperty("address");

                return GetBestLocationName(address);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Reverse geocoding failed: {ex.Message}");

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

    }
}
