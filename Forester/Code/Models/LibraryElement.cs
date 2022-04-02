using Avalonia.Media.Imaging;
using Forester.Models.API;

namespace Forester.Models
{
    public struct LibraryElement
    { 
        public AppConfig appConfig { get; set; }
        public Application app { get; set; }
        public Bitmap profilePicture { get; set; }
        public Bitmap backgroundPicture { get; set; }
    }
}
