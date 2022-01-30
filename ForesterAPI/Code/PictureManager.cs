using System.IO;
using System.Drawing;

namespace ForesterAPI
{
    public static class PictureManager
    {
        public static (bool, bool) CheckIfExist(string username, Folder folder)
        {
            bool profilePicture, backgroundPicture;

            profilePicture = File.Exists($"./Database/Picutres/{folder}/{username}/profilePicture.png");
            backgroundPicture = File.Exists($"./Database/Picutres/{folder}/{username}/backgroundPicture.png");

            return (profilePicture, backgroundPicture);
        }

        public static byte[] GetImage(string username, Picture type, Folder folder) => File.ReadAllBytes($"./Database/Picutres/{folder}/{username}/{type}Picture.png");

        public static void UpdateImage(string username, Picture type, Folder folder, byte[] data) => File.WriteAllBytes($"./Database/Picutres/{folder}/{username}/{type}Picture.png", data);

        public enum Picture
        {
            profile,
            background,
        }

        public enum Folder
        {
            Accounts,
            Applications
        }
    }
}
