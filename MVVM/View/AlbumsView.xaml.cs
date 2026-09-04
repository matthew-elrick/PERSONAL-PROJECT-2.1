using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using PERSONAL_PROJECT_2.MVVM.Model;
using PERSONAL_PROJECT_2.MVVM.ViewModel;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PERSONAL_PROJECT_2.MVVM.View
{
    public partial class AlbumsView : UserControl
    {
        public AlbumsView()
        {
            InitializeComponent();
        }

        private void Album_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border &&
                border.DataContext is PhotoGroup group)
            {
                if (DataContext is AlbumsViewModel albumsViewModel)
                {
                    albumsViewModel.OpenAlbum(group);
                }
            }
        }
        /*private void AlbumImage_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not Image image)
                return;

            if (image.DataContext is not PhotoGroup group)
                return;

            var photo = group.FirstPhoto;

            if (photo == null || !File.Exists(photo.PhotoPath))
                return;

            image.Source = LoadPhotoWithOrientation(photo.PhotoPath);
        }

        private BitmapSource LoadPhotoWithOrientation(string photoPath)
        {
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri(photoPath);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            bitmap.EndInit();

            bitmap.Freeze();

            var metadata = MetadataExtractor.ImageMetadataReader.ReadMetadata(photoPath);

            var exif = metadata
                .OfType<MetadataExtractor.Formats.Exif.ExifIfd0Directory>()
                .FirstOrDefault();

            if (exif == null)
                return bitmap;

            if (!exif.TryGetInt32(
                MetadataExtractor.Formats.Exif.ExifDirectoryBase.TagOrientation,
                out int orientation))
            {
                return bitmap;
            }

            switch (orientation)
            {
                case 1:
                    return bitmap;

                case 2:
                    return new TransformedBitmap(
                        bitmap,
                        new ScaleTransform(-1, 1));

                case 3:
                    return new TransformedBitmap(
                        bitmap,
                        new RotateTransform(180));

                case 4:
                    return new TransformedBitmap(
                        bitmap,
                        new ScaleTransform(1, -1));

                case 5:
                    return new TransformedBitmap(
                        bitmap,
                        new TransformGroup
                        {
                            Children =
                            {
                        new ScaleTransform(-1, 1),
                        new RotateTransform(270)
                            }
                        });

                case 6:
                    return new TransformedBitmap(
                        bitmap,
                        new RotateTransform(90));

                case 7:
                    return new TransformedBitmap(
                        bitmap,
                        new TransformGroup
                        {
                            Children =
                            {
                        new ScaleTransform(-1, 1),
                        new RotateTransform(90)
                            }
                        });

                case 8:
                    return new TransformedBitmap(
                        bitmap,
                        new RotateTransform(270));

                default:
                    return bitmap;
            }
        }*/
    }
}