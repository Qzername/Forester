using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Code.Models.Pictures
{
    public struct ElementPicutres
    {
        public bool IsDefaultProfilePicture;
        public Bitmap ProfilePicture { get; set; }
        public bool IsDefaultBackgroundPicture;
        public Bitmap BackgroundPicture { get; set; }
    }
}
