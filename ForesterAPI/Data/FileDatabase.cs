using ForesterAPI.Data.Connection;

namespace ForesterAPI.Data
{
    public class FileDatabase
    {
        FileApplicationManager fileApplicationManager;
        PictureManager pictureManager;

        public FileDatabase(FileApplicationManager FileApplicationManager, PictureManager PictureManager)
        {
            fileApplicationManager = FileApplicationManager;
            pictureManager = PictureManager;
        }

        public void Rename(string OldName, string NewName)
        {
            fileApplicationManager.Rename(OldName, NewName);
            pictureManager.Rename(OldName, NewName);
        }
    }
}
