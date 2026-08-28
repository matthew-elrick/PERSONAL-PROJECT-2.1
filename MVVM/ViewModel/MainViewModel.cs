using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PERSONAL_PROJECT_2.Core;

namespace PERSONAL_PROJECT_2.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {

        public RelayCommand UploadPhotoViewCommand { get; set; }

        public RelayCommand MapExplorerViewCommand { get; set; }

        public UploadPhotoViewModel UploadPhotoVM { get; set; }
        public MapExplorerViewModel MapExplorerVM { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }


        public MainViewModel()
        {
            UploadPhotoVM = new UploadPhotoViewModel();
            MapExplorerVM = new MapExplorerViewModel();

            UploadPhotoVM.PhotoUploaded += MapExplorerVM.AddPhoto;

            foreach (var photo in UploadPhotoVM.Photos)
            {
                MapExplorerVM.AddPhoto(photo);
            }

            CurrentView = UploadPhotoVM;

            UploadPhotoViewCommand = new RelayCommand(o =>
            {
                CurrentView = UploadPhotoVM;
            });

            MapExplorerViewCommand = new RelayCommand(o =>
            {
                CurrentView = MapExplorerVM;
            });
        }
    }
}