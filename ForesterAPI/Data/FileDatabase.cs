using ForesterAPI.Data.Connection;

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
            pictureManager.Rename(oldName, newName);
        }
    }
}
