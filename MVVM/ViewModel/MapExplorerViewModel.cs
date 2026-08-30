using PERSONAL_PROJECT_2.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Text.Json;

namespace PERSONAL_PROJECT_2.MVVM.ViewModel
{
    internal class MapExplorerViewModel
    {
        public event Action<PhotoInfo> PhotoReceived;
        public List<PhotoInfo> Photos { get; } = new();
        public List<PhotoGroup> PhotoGroups { get; } = new();

        public event Action<PhotoInfo> PhotoNeedsLocation;

        private readonly Queue<PhotoInfo> _pendingPhotos = new();
        public PhotoInfo? PendingPhoto =>
            _pendingPhotos.Count > 0
                ? _pendingPhotos.Peek()
                : null;
        private readonly string _jsonPath = Path.Combine(
            Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData),
            "PERSONAL_PROJECT",
            "photos.json");

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
            SavePhotos();
            PhotoReceived?.Invoke(photo);
        }

        public void StartLocationPicking(PhotoInfo photo)
        {
            _pendingPhotos.Enqueue(photo);

            System.Diagnostics.Debug.WriteLine(
                $"Queued photo for manual location: {photo.Filename}");

            // Only start the first photo.
            if (_pendingPhotos.Count == 1)
            {
                PhotoNeedsLocation?.Invoke(photo);
            }
        }

        public PhotoInfo? GetNextPendingPhoto()
        {
            if (_pendingPhotos.Count == 0)
                return null;

            return _pendingPhotos.Peek();
        }

        public void CompletePendingPhoto()
        {
            if (_pendingPhotos.Count > 0)
            {
                _pendingPhotos.Dequeue();
            }
        }
        private void SavePhotos()
        {
            try
            {
                string? directory = Path.GetDirectoryName(_jsonPath);

                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonSerializer.Serialize(
                    Photos,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                File.WriteAllText(_jsonPath, json);

                System.Diagnostics.Debug.WriteLine(
                    $"Saved {Photos.Count} photos to JSON.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error saving photos: {ex.Message}");
            }
        }
    }
}
