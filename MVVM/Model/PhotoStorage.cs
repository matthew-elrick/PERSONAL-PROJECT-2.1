using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PERSONAL_PROJECT_2.MVVM.Model
{
    public static class PhotoStorage
    {
        private static readonly string AppFolder =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "PERSONAL_PROJECT");

        public static readonly string PhotosFolder =
            Path.Combine(AppFolder, "photos");

        private static readonly string DatabaseFile =
            Path.Combine(AppFolder, "photos.json");

        private static readonly JsonSerializerOptions JsonOptions =
            new JsonSerializerOptions
            {
                WriteIndented = true
            };

        public static List<PhotoInfo> LoadPhotos()
        {
            try
            {
                Directory.CreateDirectory(PhotosFolder);

                if (!File.Exists(DatabaseFile))
                {
                    return new List<PhotoInfo>();
                }

                string json = File.ReadAllText(DatabaseFile);

                var photos =
                    JsonSerializer.Deserialize<List<PhotoInfo>>(
                        json,
                        JsonOptions)
                    ?? new List<PhotoInfo>();

                // Remove JSON entries for photos that
                // no longer exist in the photos folder.
                int originalCount = photos.Count;

                photos = photos
                    .Where(photo =>
                        File.Exists(
                            Path.Combine(
                                PhotosFolder,
                                photo.Filename)))
                    .ToList();

                // Save the cleaned list if anything was removed.
                if (photos.Count != originalCount)
                {
                    SavePhotos(photos);

                    System.Diagnostics.Debug.WriteLine(
                        $"Removed {originalCount - photos.Count} " +
                        $"missing photos from photos.json.");
                }

                return photos;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Could not load photos: {ex}");

                return new List<PhotoInfo>();
            }
        }

        public static void SavePhotos(
            IEnumerable<PhotoInfo> photos)
        {
            try
            {
                Directory.CreateDirectory(PhotosFolder);

                string json =
                    JsonSerializer.Serialize(
                        photos,
                        JsonOptions);

                File.WriteAllText(
                    DatabaseFile,
                    json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Could not save photos: {ex}");
            }
        }
    }
}