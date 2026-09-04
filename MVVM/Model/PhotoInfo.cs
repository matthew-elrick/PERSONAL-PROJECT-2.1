using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Text.Json.Serialization;

namespace PERSONAL_PROJECT_2.MVVM.Model
{
    public class PhotoInfo
    {
        public string PhotoPath
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),
                    "PERSONAL_PROJECT",
                    "photos",
                    Filename);
            }
        }

        public string Filename { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LocationName { get; set; }

        [JsonIgnore]
        public BitmapSource DisplayImage
        {
            get
            {
                return LoadPhotoWithOrientation(PhotoPath);
            }
        }

        private static BitmapSource LoadPhotoWithOrientation(string photoPath)
        {
            if (!File.Exists(photoPath))
                return null;

            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri(photoPath);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            bitmap.EndInit();
            bitmap.Freeze();

            try
            {
                var metadata = ImageMetadataReader.ReadMetadata(photoPath);

                var exif = metadata
                    .OfType<ExifIfd0Directory>()
                    .FirstOrDefault();

                if (exif == null)
                    return bitmap;

                if (!exif.TryGetInt32(
                    ExifDirectoryBase.TagOrientation,
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
            }
            catch
            {
                return bitmap;
            }
        }
    }
}