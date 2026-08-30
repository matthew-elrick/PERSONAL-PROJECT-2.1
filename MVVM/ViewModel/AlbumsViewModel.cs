using System;
using System.Collections.Generic;
using System.Text;
using PERSONAL_PROJECT_2.MVVM.Model;

namespace PERSONAL_PROJECT_2.MVVM.ViewModel
{
    class AlbumsViewModel
    {
        public List<PhotoGroup> Albums { get; }

        public AlbumsViewModel(List<PhotoGroup> photoGroups)
        {
            Albums = photoGroups;
        }
    }
}
