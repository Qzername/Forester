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

        public string GetVersion() => File.ReadAllText($"{DatabasePrefix}version.json");
        public string GetChecksum() => File.ReadAllText($"{DatabasePrefix}checksum.json");

        public byte[] GetForesterFiles(Dictionary<string, string> doNotInclude)
        {
            var checksum = JsonManager.Deserialize<Dictionary<string, string>>(File.ReadAllText($"{DatabasePrefix}checksum.json"));

            return fileConfigurator.ExtractFiles($"{DatabasePrefix}forester.zip", checksum, doNotInclude, false);
        }

        public void SetForesterFiles(string version, IFormFile file)
        {
            if (File.Exists($"{DatabasePrefix}forester.zip"))
                File.Delete($"{DatabasePrefix}forester.zip");

            FileStream stream = new FileStream($"{DatabasePrefix}forester.zip", FileMode.Create);
            file.CopyTo(stream);
            stream.Close();

            ZipArchive zip = ZipFile.Open($"{DatabasePrefix}forester.zip", ZipArchiveMode.Read);

            var checksum = fileConfigurator.CreateChecksum(zip);

            File.WriteAllText($"{DatabasePrefix}checksum.json", fileConfigurator.DictonaryToJson(checksum));
            File.WriteAllText($"{DatabasePrefix}version.json", "{ \"Version\":\"" + version + "\"}");

            zip.Dispose();
        }
    }
}
