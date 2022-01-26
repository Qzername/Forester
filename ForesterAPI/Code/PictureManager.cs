using System.IO;
using System.Drawing;

namespace ForesterAPI
{
    public static class PictureManager
    {
        public static (bool, bool) CheckIfExist(string username)
        {
            bool profilePicture, backgroundPicture;

            profilePicture = File.Exists($"./Database/Picutres/{username}/profilePicture.png");
            backgroundPicture = File.Exists($"./Database/Picutres/{username}/backgroundPicture.png");

            return (profilePicture, backgroundPicture);
        }

        public static byte[] GetImage(string username, Picture type) => File.ReadAllBytes($"./Database/Picutres/{username}/{type}Picture.png");

        public static void UpdateImage(string username, Picture type, byte[] data) => File.WriteAllBytes($"./Database/Picutres/{username}/{type}Picture.png", data);

        public enum Picture
        {
            profile,
            background,
        }
    }
}
