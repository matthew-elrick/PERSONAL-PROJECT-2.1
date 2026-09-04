using PERSONAL_PROJECT_2.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Text.Json;
using System.Net.Http;

namespace PERSONAL_PROJECT_2.MVVM.ViewModel
{
    internal class MapExplorerViewModel
    {
        public event Action<PhotoInfo> PhotoReceived;
        public List<PhotoInfo> Photos { get; } = new();
        public List<PhotoGroup> PhotoGroups { get; } = new();

        public event Action<PhotoInfo> PhotoNeedsLocation;

        private readonly Queue<PhotoInfo> _pendingPhotos = new();

        private const double GroupDistanceMeters = 1000;

        private readonly HttpClient _httpClient = new HttpClient();
        private readonly Dictionary<string, string> _locationCache = new();

        public PhotoInfo? PendingPhoto =>
            _pendingPhotos.Count > 0
                ? _pendingPhotos.Peek()
                : null;
        private readonly string _jsonPath = Path.Combine(
            Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData),
            "PERSONAL_PROJECT",
            "photos.json");

        public MapExplorerViewModel()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "PERSONAL_PROJECT_2.1/1.0");
        }

        public async void AddPhoto(PhotoInfo photo)
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

            // Get the location BEFORE creating the album/group
            if (string.IsNullOrWhiteSpace(photo.LocationName))
            {
                photo.LocationName = await GetLocationName(
                    photo.Latitude,
                    photo.Longitude);

                System.Diagnostics.Debug.WriteLine(
                    $"Location found for {photo.Filename}: {photo.LocationName}");
            }

            Photos.Add(photo);

            AddPhotoToGroup(photo);

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

        private void AddPhotoToGroup(PhotoInfo photo)
        {
            PhotoGroup? nearbyGroup = null;

            foreach (var group in PhotoGroups)
            {
                var firstPhoto = group.FirstPhoto;

                double distance = CalculateDistanceMeters(
                    photo.Latitude,
                    photo.Longitude,
                    firstPhoto.Latitude,
                    firstPhoto.Longitude);

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

                return;
            }

            var newGroup = new PhotoGroup();
            newGroup.Photos.Add(photo);

            PhotoGroups.Add(newGroup);

            System.Diagnostics.Debug.WriteLine(
                $"Created new photo group for {photo.Filename}");
        }
        private static double CalculateDistanceMeters(
            double latitude1,
            double longitude1,
            double latitude2,
            double longitude2)
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
        /*private async Task LoadLocationName(PhotoInfo photo)
        {
            if (!string.IsNullOrWhiteSpace(photo.LocationName))
                return;

            string locationName = await GetLocationName(
                photo.Latitude,
                photo.Longitude);

            photo.LocationName = locationName;

            System.Diagnostics.Debug.WriteLine(
                $"Location found for {photo.Filename}: {locationName}");
        }*/
        public async Task<string> GetLocationName( double latitude, double longitude)
        {
            string cacheKey = $"{latitude:F5},{longitude:F5}";

            if (_locationCache.TryGetValue(
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
                    await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                string json =
                    await response.Content.ReadAsStringAsync();

                using JsonDocument document =
                    JsonDocument.Parse(json);

                JsonElement root = document.RootElement;

                if (!root.TryGetProperty(
                    "address",
                    out JsonElement address))
                {
                    return "Unknown location";
                }

                string location =
                    GetBestLocationName(address);

                _locationCache[cacheKey] = location;

                return location;
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
