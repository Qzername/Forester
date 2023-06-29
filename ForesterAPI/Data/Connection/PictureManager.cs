using ForesterAPI.Models.Picture;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ForesterAPI.Data.Connection
{
    public class PictureManager
    {
        const string Prefix = "./ForesterDatabase/Pictures/";

        public void Rename(ObjectType objectType, string oldName, string newName)
        {
            if(Directory.Exists(GetDirectoryPath(oldName, objectType)))
                Directory.Move(GetDirectoryPath(oldName, objectType), GetDirectoryPath(newName, objectType));
        }

        public void Delete(ObjectType objectType, string name)
        {
            if (Directory.Exists(GetDirectoryPath(name, objectType)))
                Directory.Delete(GetDirectoryPath(name, objectType), true);
        }
        
        public byte[] Get(ObjectType objectType, string name, PictureType pictureType) => File.ReadAllBytes(GetPathToFile(objectType, name, pictureType);

        public void Update(ObjectType objectType, string name, PictureType pictureType, byte[] picture)
        {
            string directoryPath = GetDirectoryPath(name, objectType);
            string filePath = GetPathToFile(objectType, name, pictureType);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            if (File.Exists(filePath))
                File.Delete(filePath);

            File.WriteAllBytes(filePath, picture);
        }

        public bool DoesExist(ObjectType objectType, string name, PictureType pictureType) => File.Exists(GetPathToFile(objectType, name, pictureType));

        string GetPathToFile(ObjectType objectType, string name, PictureType pictureType) =>
            $"{GetDirectoryPath(name, objectType)}/{name}/" +
            $"{Enum.GetName(typeof(PictureType), pictureType)}.png";

        string GetDirectoryPath(string name, ObjectType objectType) => Prefix + Enum.GetName(typeof(ObjectType), objectType) + $"/{name}/";
    }
}