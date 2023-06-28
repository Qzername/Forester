namespace ForesterAPI.Data.Connection
{
    public class PictureManager
    {
        const string Prefix = "./ForesterDatabase/Pictures/";

        public void Rename(string oldName, string newName)
        {
            if(Directory.Exists(GetDirectoryPath(oldName)))
                Directory.Move(GetDirectoryPath(oldName), GetDirectoryPath(newName));
        }

        string GetDirectoryPath(string name) => Prefix + name + "/";
    }
}
