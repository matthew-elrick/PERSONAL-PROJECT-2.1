using PERSONAL_PROJECT_2.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PERSONAL_PROJECT_2.MVVM.ViewModel
{
    class AlbumDetailViewModel
    {
        public ObservableCollection<PhotoInfo> Photos { get; }

        public string LocationName =>
            Photos.FirstOrDefault()?.LocationName ?? "Album";

        private readonly PhotoGroup _group;

        private readonly string _jsonPath = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "PERSONAL_PROJECT",
            "photos.json");

        public AlbumDetailViewModel(PhotoGroup group)
        {
            _group = group;

            Photos = new ObservableCollection<PhotoInfo>(
                group.Photos);
        }

        public event Action BackRequested;
        public event Action<PhotoInfo> PhotoDeleted;
        public event Action AlbumEmpty;

        public void GoBack()
        {
            BackRequested?.Invoke();
        }

        public void DeletePhoto(PhotoInfo photo)
        {
            if (photo == null)
                return;

            try
            {
                if (File.Exists(photo.PhotoPath))
                {
                    File.Delete(photo.PhotoPath);

                    System.Diagnostics.Debug.WriteLine(
                        $"Deleted photo: {photo.PhotoPath}");
                }

                string thumbnailDirectory = Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),
                    "PERSONAL_PROJECT",
                    "thumbnails");

                string thumbnailPath = Path.Combine(
                    thumbnailDirectory,
                    Path.GetFileNameWithoutExtension(photo.Filename) + ".png");

                if (File.Exists(thumbnailPath))
                {
                    File.Delete(thumbnailPath);

                    System.Diagnostics.Debug.WriteLine(
                        $"Deleted thumbnail: {thumbnailPath}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Could not delete photo or thumbnail: {ex.Message}");

                return;
            }

            _group.Photos.Remove(photo);
            Photos.Remove(photo);

            SavePhotos();
            PhotoDeleted?.Invoke(photo);

            if (Photos.Count == 0)
            {
                AlbumEmpty?.Invoke();
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

                List<PhotoInfo> allPhotos = new();

                if (File.Exists(_jsonPath))
                {
                    string json = File.ReadAllText(_jsonPath);

                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        allPhotos =
                            JsonSerializer.Deserialize<List<PhotoInfo>>(json)
                            ?? new List<PhotoInfo>();
                    }
                }

                allPhotos = allPhotos
                    .Where(p => File.Exists(p.PhotoPath))
                    .ToList();

                string updatedJson = JsonSerializer.Serialize(
                    allPhotos,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                File.WriteAllText(_jsonPath, updatedJson);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error updating photos.json: {ex.Message}");
            }
        }
    }
}
