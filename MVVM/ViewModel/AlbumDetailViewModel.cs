using PERSONAL_PROJECT_2.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PERSONAL_PROJECT_2.MVVM.ViewModel
{
    class AlbumDetailViewModel
    {
        public List<PhotoInfo> Photos { get; }

        public string LocationName =>
            Photos.FirstOrDefault()?.LocationName ?? "Album";

        public AlbumDetailViewModel(PhotoGroup group)
        {
            Photos = group.Photos;
        }

        public event Action BackRequested;
        public void GoBack()
        {
            BackRequested?.Invoke();
        }
    }
}