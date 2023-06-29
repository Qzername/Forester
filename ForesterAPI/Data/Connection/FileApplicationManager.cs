using ForesterAPI.Tools;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace ForesterAPI.Data.Connection
{
    public class FileApplicationManager
    {
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

            var checksum = CreateChecksum(zip);

            zip.Dispose();

            File.WriteAllText($"{directoryPath}checksum.json", DictonaryToJson(checksum));
        }

        /// <summary>
        /// returns value in .zip file
        /// </summary>
        public byte[] ExtractFiles(string name, Dictionary<string,string> doNotInclude)
        {
            string directoryPath = GetDirectoryPath(name);

            var config = JsonManager.Deserialize<Dictionary<string, string>>(File.ReadAllText($"{directoryPath}checksum.json"));

            List<string> filesToRemove = new List<string>();

            //usunięcie plików bez zmian, oznaczenie niepotrzebnych jako do usuniecia
            foreach (KeyValuePair<string, string> key in doNotInclude)
                if (config.ContainsKey(key.Key) && config[key.Key] == key.Value)
                    filesToRemove.Add(key.Key);
                else
                    config[key.Key] = "REMOVE";

            //Skopiowanie zipa do tempa, usunięcie plików, przesłanie dalej
            string pathToTemp = $"{DatabasePrefix}Temp/{new Random().Next(0, 100000)}/";
            string pathToAppZip = $"{directoryPath}app.zip";

            Directory.CreateDirectory(pathToTemp);
            File.Copy(pathToAppZip, pathToTemp + "app.zip");

            using (ZipArchive archive = ZipFile.Open(pathToTemp + "app.zip", ZipArchiveMode.Update))
            {
                var entriesToRemove = archive.Entries.Where(x => filesToRemove.Contains(x.FullName)).ToList();

                for (int i = 0; i < entriesToRemove.Count; i++)
                    entriesToRemove[i].Delete();

                archive.CreateEntry("ForesterConfig/");
                var entry = archive.CreateEntry("ForesterConfig/checksum.json");

                using (Stream stream = entry.Open())
                {
                    var sw = new StreamWriter(stream);
                    sw.Write(JsonManager.Serialize(config));
                    sw.Flush();
                    sw.Close();
                }
            } 

            byte[] data = File.ReadAllBytes(pathToTemp + "app.zip");

            Directory.Delete(pathToTemp, true);

            return data;
        }

        public string GetChecksum(string name) => File.ReadAllText(GetDirectoryPath(name) + "checksum.json");

        public bool DoesExist(string name) => File.Exists(GetDirectoryPath(name) + "checksum.json");

        Dictionary<string, string> CreateChecksum(ZipArchive zip)
        {
            Dictionary<string, string> json = new Dictionary<string, string>();

            var md5 = MD5.Create();

            foreach (var entry in zip.Entries)
            {
                var file = entry.Open();
                var hashMD5 = md5.ComputeHash(file);
                file.Close();

                json.Add(entry.FullName, BitConverter.ToString(hashMD5).Replace("-", "").ToLowerInvariant());
            }

            return json;
        }
        string DictonaryToJson(Dictionary<string, string> dict)
        {
            var entries = dict.Select(d => string.Format("\"{0}\": \"{1}\"", d.Key, d.Value));
            return "{" + string.Join(",", entries) + "}";
        }

        string GetDirectoryPath(string name) => Prefix + name + "/";
    }
}
