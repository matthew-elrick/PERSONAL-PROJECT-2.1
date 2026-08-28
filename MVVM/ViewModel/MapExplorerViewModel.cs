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

        public List<PhotoInfo> Photos { get; } = new();
        public void AddPhoto(PhotoInfo photo)
        {
            if (photo.Latitude == 0 &&
                photo.Longitude == 0)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Skipping {photo.Filename} - no GPS.");

                return;
            }

            System.Diagnostics.Debug.WriteLine(
                $"Photo received: {photo.Filename}");

            System.Diagnostics.Debug.WriteLine(
                $"Location: {photo.LocationName}");

            Photos.Add(photo);

            PhotoReceived?.Invoke(photo);
        }
    }
}
