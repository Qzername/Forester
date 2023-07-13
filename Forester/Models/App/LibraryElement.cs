using Avalonia.Media.Imaging;
using Forester.Models.API;

namespace Forester.Models.App
{
    public struct LibraryElement
    {
        public Application Application { get; set; }
        public Bitmap ProfilePicture { get; set; }
        public Bitmap BackgroundPicture { get; set; }
    }
}
