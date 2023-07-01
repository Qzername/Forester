using ForesterAPI.Data.Connection;

namespace ForesterAPI.Data
{
    public class ForesterFileDatabase
    {
        ForesterFileManager foresterFileManager;

        public ForesterFileDatabase(ForesterFileManager foresterFileManager)
        {
            this.foresterFileManager = foresterFileManager;
        }

        public string GetVersion() =>foresterFileManager.GetVersion();
        public string GetChecksum() =>foresterFileManager.GetChecksum();
        public byte[] GetForesterFiles(Dictionary<string, string> doNotInclude) => foresterFileManager.GetForesterFiles(doNotInclude);
        public void SetForesterFiles(string version, IFormFile file) => foresterFileManager.SetForesterFiles(version, file);
    }
}
