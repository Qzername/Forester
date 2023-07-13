using Avalonia.Media.Imaging;
using Forester.Models.API;

namespace Forester.Models.App
{
    public struct StoreElement
    {
        public bool IsInLibrary { get; set; }
        public Application Application { get; set; }
        public Account Owner { get; set; }
        public Bitmap ProfilePicture { get; set; }
        public Bitmap BackgroundPicture { get; set; }
    }
}
