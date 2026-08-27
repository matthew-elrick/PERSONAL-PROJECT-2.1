using PERSONAL_PROJECT_2.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PERSONAL_PROJECT_2.MVVM.ViewModel
{
    internal class MapExplorerViewModel
    {
        public event Action<PhotoInfo> PhotoReceived;
        public void AddPhoto(PhotoInfo photo)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Photo received: {photo.Filename}");

            System.Diagnostics.Debug.WriteLine(
                $"Latitude: {photo.Latitude}");

            System.Diagnostics.Debug.WriteLine(
                $"Longitude: {photo.Longitude}");

            PhotoReceived?.Invoke(photo);
        }
    }
}
