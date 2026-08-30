using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PERSONAL_PROJECT_2.MVVM.Model
{
    public class Album
    {
        public List<PhotoInfo> Photos { get; set; } = new();

        public PhotoInfo CoverPhoto => Photos.First();

        public string LocationName => CoverPhoto.LocationName;

        public int PhotoCount => Photos.Count;
    }
}
