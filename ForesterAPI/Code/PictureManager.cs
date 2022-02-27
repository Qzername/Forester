using System.IO;
using System.Drawing;

namespace ForesterAPI
{
    public static class PictureManager
    {
        public static (bool, bool) CheckIfExist(string username, Folder folder)
        {
            bool profilePicture, backgroundPicture;

            profilePicture = File.Exists($"./Database/Pictures/{folder}/{username}/profilePicture.png");
            backgroundPicture = File.Exists($"./Database/Pictures/{folder}/{username}/backgroundPicture.png");

            return (profilePicture, backgroundPicture);
        }

        public static byte[] GetImage(string username, Picture type, Folder folder) => File.ReadAllBytes($"./Database/Pictures/{folder}/{username}/{type}Picture.png");

        public static void UpdateImage(string username, Picture type, Folder folder, byte[] data) 
        {
            if (!File.Exists($"./Database/Pictures/{folder}/{username}/{type}Picture.png"))
            {
                Directory.CreateDirectory($"./Database/Pictures/{folder}/{username}");
            }

            File.WriteAllBytes($"./Database/Pictures/{folder}/{username}/{type}Picture.png", data); 
        }

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
