using PERSONAL_PROJECT_2.Core;
using PERSONAL_PROJECT_2.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERSONAL_PROJECT_2.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {

        public RelayCommand UploadPhotoViewCommand { get; set; }
        public RelayCommand MapExplorerViewCommand { get; set; }       
        public RelayCommand AlbumsViewCommand { get; set; }


        public UploadPhotoViewModel UploadPhotoVM { get; set; }
        public MapExplorerViewModel MapExplorerVM { get; set; }
        public AlbumsViewModel AlbumsVM { get; set; }

        public AlbumDetailViewModel AlbumDetailVM { get; set; }

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
            AlbumsVM = new AlbumsViewModel(MapExplorerVM.PhotoGroups);
            AlbumsVM.AlbumSelected += OpenAlbum;

            UploadPhotoVM.PhotoUploaded += MapExplorerVM.AddPhoto;
            UploadPhotoVM.PhotoNeedsLocation += HandlePhotoNeedsLocation;

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
            AlbumsViewCommand = new RelayCommand(o =>
            {
                CurrentView = AlbumsVM;
            });
        }

        private void HandlePhotoNeedsLocation(PhotoInfo photo)
        {
            CurrentView = MapExplorerVM;
            MapExplorerVM.StartLocationPicking(photo);
        }

        private void OpenAlbum(PhotoGroup group)
        {
            AlbumDetailVM = new AlbumDetailViewModel(group);

            AlbumDetailVM.BackRequested += () =>
            {
                CurrentView = AlbumsVM;
            };

            CurrentView = AlbumDetailVM;
        }
    }
}