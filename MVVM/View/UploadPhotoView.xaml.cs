using MetadataExtractor;
using MetadataExtractor.Formats.Apple;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using Microsoft.Win32;
using PERSONAL_PROJECT_2.CustomPhotoControls;
using PERSONAL_PROJECT_2.MVVM.Model;
using PERSONAL_PROJECT_2.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Diagnostics;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PERSONAL_PROJECT_2.MVVM.View
{
    /// <summary>
    /// Interaction logic for UploadPhotoView.xaml
    /// </summary>
    public partial class UploadPhotoView : UserControl
    {
        public UploadPhotoView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process_Photo();
        }

        private void Rectangle_Drop(object sender, DragEventArgs e)
        {
            Process_Photo();
        }

        public void Process_Photo()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Multiselect = true };
            openFileDialog.Filter = "Image Files (*.gif,*.jpg,*.jpeg,*.bmp,*.png)|*.gif;*.jpg;*.jpeg;*.bmp;*.png";
            openFileDialog.FilterIndex = 1;
            bool? response = openFileDialog.ShowDialog();
            if (response == true)
            {
                string[] files = openFileDialog.FileNames;

                for (int i = 0; i < files.Length; i++)
                {
                    string filename = System.IO.Path.GetFileName(files[i]);
                    double? latitude = null;
                    double? longitude = null;
                    FileInfo fileInfo = new FileInfo(files[i]);
                    UploadingFilesList.Items.Add(new FileDeatil()
                    {
                        FileName = filename,
                        FileSize = string.Format("{0} {1}", (fileInfo.Length / 1.049e+6).ToString("0.0"), "Mb"),
                        UploadProgress = 100

                    });

                    //create folder to store photos
                    string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    string dir = System.IO.Path.Combine(localAppDataPath, "PERSONAL_PROJECT", "photos");
                    if (!System.IO.Directory.Exists(dir))
                    {
                        System.IO.Directory.CreateDirectory(dir);
                    }

                    string sourcePath = files[i];
                    string filenameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(filename);
                    string extension = System.IO.Path.GetExtension(filename);
                    string targetPath = System.IO.Path.Combine(dir, filename);

                    int counter = 1;

                    while (File.Exists(targetPath))
                    {
                        string newFilename = $"{filenameWithoutExtension}_{counter}{extension}";
                        targetPath = System.IO.Path.Combine(dir, newFilename);
                        counter++;
                    }
                    File.Copy(sourcePath, targetPath);

                    string storedFilename = System.IO.Path.GetFileName(targetPath);

                    try
                    {
                        var gps = ImageMetadataReader.ReadMetadata(files[i]).OfType<GpsDirectory>().FirstOrDefault();

                        if (gps == null)
                        {
                            System.Diagnostics.Debug.WriteLine("No GPS found.");
                        }
                        else if (gps.GetGeoLocation() is GeoLocation location)
                        {

                            latitude = location.Latitude;
                            longitude = location.Longitude;

                            System.Diagnostics.Debug.WriteLine(files[i]);
                            System.Diagnostics.Debug.WriteLine(
                                $"Latitude: {location.Latitude}");
                            System.Diagnostics.Debug.WriteLine(
                                $"Longitude: {location.Longitude}");
                            System.Diagnostics.Debug.WriteLine("-----------------------------------");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("GPS found, but no coordinates.");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }

                    PhotoInfo photoInfo = new PhotoInfo
                    {
                        Filename = storedFilename,
                        Latitude = latitude,
                        Longitude = longitude
                    };
                    var viewModel = DataContext as UploadPhotoViewModel;
                    viewModel?.NotifyPhotoUploaded(photoInfo);
                }
            }

        }
    }
}
