using PERSONAL_PROJECT_2.MVVM.Model;
using PERSONAL_PROJECT_2.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Versioning;

namespace PERSONAL_PROJECT_2.MVVM.View
{
    public partial class MapExplorerView
    {
        public MapExplorerView()
        {
            InitializeComponent();

            DataContextChanged += MapExplorerView_DataContextChanged;

            // WebView2 APIs are Windows-only. Guard registration and initialization so analyzers
            // and runtime calls are only executed on Windows.
            // Only initialize WebView2 on Windows 10.0.17763 or later.
            if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763))
            {
                MapView.NavigationCompleted += MapView_NavigationCompleted;
                MapView.CoreWebView2InitializationCompleted += MapView_CoreWebView2InitializationCompleted;
                _ = MapView.EnsureCoreWebView2Async();
            }
        }

        [SupportedOSPlatform("windows10.0.17763")]
        private async void MapView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            /*string photo = "Calabogie_test_image.jpeg";
            double latitude = 45.2633388888889;
            double longitude = -76.8122555555556;

            var photoData = new
            {
                filename = photo,
                latitude = latitude,
                longitude = longitude
            };

            string json = JsonSerializer.Serialize(photoData);

            System.Diagnostics.Debug.WriteLine(json);
            await MapView.ExecuteScriptAsync($"addPhotoMarker({json});");*/
        }
        [SupportedOSPlatform("windows10.0.17763")]
        private void MapView_CoreWebView2InitializationCompleted(
        object sender,
        Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                return;
            }

            // Folder containing map.html
            string htmlFolder =
                System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "HTML");

            // Folder containing uploaded photos
            string localAppDataPath =
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string photoFolder =
                System.IO.Path.Combine(
                    localAppDataPath,
                    "PERSONAL_PROJECT",
                    "photos");

            // Make the HTML folder accessible through a normal web origin
            MapView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "app.example",
                htmlFolder,
                Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);

            // Make the photos accessible through photos.example
            MapView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "photos.example",
                photoFolder,
                Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);
        }

        private void MapExplorerView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is MapExplorerViewModel viewModel)
            {
                viewModel.PhotoReceived += ViewModel_PhotoReceived;
            }
        }
        private async void ViewModel_PhotoReceived(PhotoInfo photo)
        {
        }
    }

}

