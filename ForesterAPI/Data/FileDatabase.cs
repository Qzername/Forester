using ForesterAPI.Data.Connection;
using ForesterAPI.Models.Picture;

namespace ForesterAPI.Data
{
    public class FileDatabase
    {
        FileApplicationManager fileApplicationManager;
        PictureManager pictureManager;

        public FileDatabase(FileApplicationManager fileApplicationManager, PictureManager pictureManager)
        {
            this.fileApplicationManager = fileApplicationManager;
            this.pictureManager = pictureManager;
        }

        public void Rename(string oldName, string newName)
        {
            fileApplicationManager.Rename(oldName, newName);
            pictureManager.Rename(ObjectType.Application, oldName, newName);
        }

        public void Delete(string application) 
        { 
            fileApplicationManager.Delete(application);
            pictureManager.Delete(ObjectType.Application, application);
        }

        // --- File Application Manager ---
        public void RenameApplication(string oldName, string newName) => fileApplicationManager.Rename(oldName, newName);
        public void DeleteApplication(string applicationName) => fileApplicationManager.Delete(applicationName);
        public void CreateApplication(string applicationName, IFormFile file) => fileApplicationManager.Create(applicationName, file);
        public byte[] GetApplicationFiles(string applicationName) => fileApplicationManager.ExtractFiles(applicationName, new Dictionary<string, string>());
        public byte[] ExtractApplicationFiles(string applicationName, Dictionary<string, string> doNotInclude) => fileApplicationManager.ExtractFiles(applicationName, doNotInclude); 
        public string GetChecksum(string applicationName) => fileApplicationManager.GetChecksum(applicationName);
        public bool DoesApplicationExist(string applicationName) => fileApplicationManager.DoesExist(applicationName);

        // --- Picture Manager ---
        public void RenamePicture(ObjectType objectType, string oldName, string newName) => pictureManager.Rename(objectType, oldName, newName);    
        public void DeletePicture(ObjectType objectType, string name) => pictureManager.Delete(objectType, name);
        public byte[] GetPicture(ObjectType objectType, string name, PictureType pictureType) => pictureManager.Get(objectType, name, pictureType;
        public void UpdatePicture(ObjectType objectType, string name, PictureType pictureType, byte[] picture) => pictureManager.Update(objectType, name, pictureType, picture);
        public bool DoesPictureExist(ObjectType objectType, string name, PictureType pictureType) => pictureManager.DoesExist(objectType, name, pictureType);
    }
}
