using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using PERSONAL_PROJECT_2.MVVM.Model;
using PERSONAL_PROJECT_2.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PERSONAL_PROJECT_2.MVVM.View
{
    public partial class MapExplorerView : UserControl
    {
        private Mapsui.Map map;
        private MemoryLayer photoLayer;

        private MapExplorerViewModel viewModel;

        public MapExplorerView()
        {
            InitializeComponent();

            map = new Mapsui.Map();
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            photoLayer = new MemoryLayer("Photos")
            {
                Features = new List<IFeature>()
            };

            map.Layers.Add(photoLayer);
            mapControl.Map = map;

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

            string localAppDataPath =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData);

            string photoPath = Path.Combine(
                localAppDataPath,
                "PERSONAL_PROJECT",
                "photos",
                photo.Filename);

            if (!File.Exists(photoPath))
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Photo file not found: {photoPath}");

                return;
            }

            var point = SphericalMercator.FromLonLat(
                photo.Longitude.Value,
                photo.Latitude.Value);

            var feature = new PointFeature(
                point.x,
                point.y);

            feature["Photo"] = photo;

            var imageStyle = new ImageStyle
            {
                Image = $"file://{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/PERSONAL_PROJECT/photos/{photo.Filename}",
                SymbolScale = 0.05
            };

            feature.Styles.Add(imageStyle);
            var features = photoLayer.Features.ToList();
            features.Add(feature);
            photoLayer.Features = features;
            photoLayer.DataHasChanged();

            System.Diagnostics.Debug.WriteLine(
                $"Added photo to map: {photo.Filename}");
            System.Diagnostics.Debug.WriteLine(
                $"Photo path: {photoPath}");
            System.Diagnostics.Debug.WriteLine(
                $"Latitude: {photo.Latitude}");
            System.Diagnostics.Debug.WriteLine(
                $"Longitude: {photo.Longitude}");
        }
    }
}