using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PERSONAL_PROJECT_2.MVVM.Model;

namespace PERSONAL_PROJECT_2.MVVM.ViewModel
{
    internal class UploadPhotoViewModel
    {
        public event Action<PhotoInfo> PhotoUploaded;

        public void NotifyPhotoUploaded(PhotoInfo photo)
        {
            PhotoUploaded?.Invoke(photo);
        }
    }
}
