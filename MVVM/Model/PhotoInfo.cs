using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
