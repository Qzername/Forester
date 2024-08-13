using ForesterAPI.Services;
using ForesterAPI.Tools;
using System;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace ForesterAPI.Data.Connection
{
    public class FileApplicationManager
    {
        ApplicationDatabase applicationDatabase;
        FileConfigurator fileConfigurator;

        public FileApplicationManager(ApplicationDatabase applicationDatabase, FileConfigurator fileConfigurator)
        {
            this.applicationDatabase = applicationDatabase;
            this.fileConfigurator = fileConfigurator;
        }

        const string DatabasePrefix = "./ForesterDatabase/";
        const string Prefix = $"{DatabasePrefix}Applications/";

        public void Rename(string oldName, string newName)
        {
            if (Directory.Exists(GetDirectoryPath(oldName)))
                Directory.Move(GetDirectoryPath(oldName), GetDirectoryPath(newName));
        }

        public void Delete(string application)
        {
            if (Directory.Exists(GetDirectoryPath(application)))
                Directory.Delete(GetDirectoryPath(application), true);
        }

        public void Create(string name, IFormFile file)
        {
            string directoryPath = GetDirectoryPath(name);

            Delete(name);
            Directory.CreateDirectory(directoryPath);

            FileStream stream = new FileStream($"{directoryPath}app.zip", FileMode.Create);
            file.CopyTo(stream);
            stream.Close();

            ZipArchive zip = ZipFile.Open($"{directoryPath}app.zip", ZipArchiveMode.Read);

            var checksum = fileConfigurator.CreateChecksum(zip);

            string jsonVersion = "{ \"Version\":\"" + applicationDatabase.Get(name).Version + "\"}";
            zip.CreateEntryFromFile($"{DatabasePrefix}Version.json", "Version.json");

            zip.Dispose();

            File.WriteAllText($"{directoryPath}checksum.json", fileConfigurator.DictonaryToJson(checksum));
            File.WriteAllText($"{DatabasePrefix}version.json", jsonVersion);
        }

        /// <summary>
        /// returns value in .zip file
        /// </summary>
        public byte[] ExtractFiles(string name, Dictionary<string,string> doNotInclude)
        {
            string directoryPath = GetDirectoryPath(name);
            string appPath = directoryPath + "app.zip";

            var checksum = JsonManager.Deserialize<Dictionary<string, string>>(File.ReadAllText($"{directoryPath}checksum.json"));

            return fileConfigurator.ExtractFiles(appPath, checksum, doNotInclude, true);
        }

        public string GetChecksum(string name) => File.ReadAllText(GetDirectoryPath(name) + "checksum.json");

        public bool DoesExist(string name) => File.Exists(GetDirectoryPath(name) + "checksum.json");

        string GetDirectoryPath(string name) => Prefix + name + "/";
    }
}
