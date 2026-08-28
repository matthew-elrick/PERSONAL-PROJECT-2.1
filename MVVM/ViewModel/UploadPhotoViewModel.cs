using System;
using System.Collections.Generic;
using System.Linq;
using PERSONAL_PROJECT_2.MVVM.Model;

namespace PERSONAL_PROJECT_2.MVVM.ViewModel
{
    internal class UploadPhotoViewModel
    {
        public event Action<PhotoInfo> PhotoUploaded;

        public List<PhotoInfo> Photos { get; private set; }

        public UploadPhotoViewModel()
        {
            Photos = PhotoStorage.LoadPhotos();
        }

        public void NotifyPhotoUploaded(PhotoInfo photo)
        {
            // Don't add the same photo twice
            if (Photos.Any(p =>
                string.Equals(
                    p.Filename,
                    photo.Filename,
                    StringComparison.OrdinalIgnoreCase)))
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Photo already exists: {photo.Filename}");

                return;
            }

            Photos.Add(photo);

            PhotoStorage.SavePhotos(Photos);

            PhotoUploaded?.Invoke(photo);
        }
    }
}