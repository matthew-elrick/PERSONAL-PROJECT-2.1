using System;
using System.Collections.Generic;
using System.Text;

namespace PERSONAL_PROJECT_2.MVVM.Model
{
    public class PhotoGroup
    {
        public List<PhotoInfo> Photos { get; } = new();

        public PhotoInfo FirstPhoto => Photos[0];
    }
}

