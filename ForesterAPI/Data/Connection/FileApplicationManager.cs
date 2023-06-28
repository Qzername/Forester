namespace ForesterAPI.Data.Connection
{
    public class FileApplicationManager
    {
        const string Prefix = "./ForesterDatabase/Applications/";

        public void Rename(string oldName, string newName)
        {
            if (Directory.Exists(GetDirectoryPath(oldName)))
                Directory.Move(GetDirectoryPath(oldName), GetDirectoryPath(newName));
        }

        string GetDirectoryPath(string name) => Prefix + name + "/";
    }
}
