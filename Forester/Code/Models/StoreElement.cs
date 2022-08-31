using Avalonia.Media.Imaging;
using Forester.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Models
{
    public struct StoreElement
    {
        public bool isInLibrary { get; set; }
        public Application app { get; set; }
        public Bitmap profilePicture { get; set; }
        public Bitmap backgroundPicture { get; set; }
    }
}
