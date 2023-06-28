namespace ForesterAPI.Data.Connection
{
    public class PictureManager
    {
        const string Prefix = "./ForesterDatabase/Pictures/";

        public void Rename(string OldName, string NewName)
        {
            if(Directory.Exists(GetDirectoryPath(OldName)))
                Directory.Move(GetDirectoryPath(OldName), GetDirectoryPath(NewName));
        }

        string GetDirectoryPath(string Name) => Prefix + Name + "/";
    }
}
