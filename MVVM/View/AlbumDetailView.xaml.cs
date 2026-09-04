using PERSONAL_PROJECT_2.MVVM.Model;
using PERSONAL_PROJECT_2.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PERSONAL_PROJECT_2.MVVM.View
{
    /// <summary>
    /// Interaction logic for AlbumDetailView.xaml
    /// </summary>
    public partial class AlbumDetailView : UserControl
    {
        public AlbumDetailView()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AlbumDetailViewModel viewModel)
            {
                viewModel.GoBack();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            if (button.DataContext is not PhotoInfo photo)
                return;

            if (DataContext is not AlbumDetailViewModel viewModel)
                return;

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete \"{photo.Filename}\"?",
                "Delete Photo",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            viewModel.DeletePhoto(photo);
        }
    }
}
