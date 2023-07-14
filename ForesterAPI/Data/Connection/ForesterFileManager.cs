using ForesterAPI.Services;
using ForesterAPI.Tools;
using System.IO.Compression;

namespace ForesterAPI.Data.Connection
{
    public class ForesterFileManager
    {
        const string DatabasePrefix = "./ForesterDatabase/Forester/";

        /*
         * Structure of files:
         * /ForesterDatabase/Forester/ - main folder
         * ./forester.zip
         * ./checksum.json 
         * ./version.json
         */

        FileConfigurator fileConfigurator;

        public ForesterFileManager(FileConfigurator fileConfigurator)
        {
            this.fileConfigurator = fileConfigurator;
        }

        public string GetVersion() => File.ReadAllText($"{DatabasePrefix}Version.json");
        public string GetChecksum() => File.ReadAllText($"{DatabasePrefix}Checksum.json");

        public byte[] GetForesterFiles(Dictionary<string, string> doNotInclude)
        {
            var checksum = JsonManager.Deserialize<Dictionary<string, string>>(File.ReadAllText($"{DatabasePrefix}Checksum.json"));

            return fileConfigurator.ExtractFiles($"{DatabasePrefix}Forester.zip", checksum, doNotInclude, false);
        }

        public void SetForesterFiles(string version, IFormFile file)
        {
            if (File.Exists($"{DatabasePrefix}Forester.zip"))
                File.Delete($"{DatabasePrefix}Forester.zip");

            FileStream stream = new FileStream($"{DatabasePrefix}Forester.zip", FileMode.Create);
            file.CopyTo(stream);
            stream.Close();

            ZipArchive zip = ZipFile.Open($"{DatabasePrefix}Forester.zip", ZipArchiveMode.Update);

            //removal of base info
            var versionEntry = zip.GetEntry("Version.json");
            versionEntry?.Delete();

            var checksumEntry = zip.GetEntry("Checksum.json");
            checksumEntry?.Delete();

            var settingsEntry = zip.GetEntry("Settings.json");
            settingsEntry?.Delete();
             
            //checksum and version
            var checksum = fileConfigurator.CreateChecksum(zip);

            string checksumString = fileConfigurator.DictonaryToJson(checksum);
            string jsonVersion = "{ \"Version\":\"" + version + "\"}";

            //adding needed files and writing them to files
            File.WriteAllText($"{DatabasePrefix}Version.json", jsonVersion);
            zip.CreateEntryFromFile($"{DatabasePrefix}Version.json", "Version.json");

            File.WriteAllText($"{DatabasePrefix}Checksum.json", checksumString);
            zip.CreateEntryFromFile($"{DatabasePrefix}Checksum.json", "Checksum.json");

            zip.Dispose();
        }
    }
}
