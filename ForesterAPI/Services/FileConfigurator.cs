using ForesterAPI.Models;
using ForesterAPI.Tools;
using System.IO.Compression;
using System.Security.Cryptography;

namespace ForesterAPI.Services
{
    public class FileConfigurator
    {
        const string DatabasePrefix = "./ForesterDatabase/";

        public Dictionary<string, string> CreateChecksum(ZipArchive zip)
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

        public string DictonaryToJson(Dictionary<string, string> dict)
        {
            var entries = dict.Select(d => string.Format("\"{0}\": \"{1}\"", d.Key, d.Value));
            return "{" + string.Join(",", entries) + "}";
        }

        public byte[] ExtractFiles(string pathToZip, Dictionary<string, string> checksum, Dictionary<string, string> doNotInclude, bool CreateChecksumFolder)
        {
            List<string> filesToRemove = new List<string>();

            //usunięcie plików bez zmian, oznaczenie niepotrzebnych jako do usuniecia
            foreach (KeyValuePair<string, string> key in doNotInclude)
                if (checksum.ContainsKey(key.Key) && checksum[key.Key] == key.Value)
                    filesToRemove.Add(key.Key);
                else
                    checksum[key.Key] = "REMOVE";

            //Skopiowanie zipa do tempa, usunięcie plików, przesłanie dalej
            string pathToTemp = $"{DatabasePrefix}Temp/{new Random().Next(0, 100000)}/";

            Directory.CreateDirectory(pathToTemp);
            File.Copy(pathToZip, pathToTemp + "app.zip");

            using (ZipArchive archive = ZipFile.Open(pathToTemp + "app.zip", ZipArchiveMode.Update))
            {
                var entriesToRemove = archive.Entries.Where(x => filesToRemove.Contains(x.FullName)).ToList();

                for (int i = 0; i < entriesToRemove.Count; i++)
                    entriesToRemove[i].Delete();

                if(CreateChecksumFolder)
                {
                    archive.CreateEntry("ForesterConfig/");
                    var entry = archive.CreateEntry("ForesterConfig/checksum.json");

                    using (Stream stream = entry.Open())
                    {
                        var sw = new StreamWriter(stream);
                        sw.Write(JsonManager.Serialize(checksum));
                        sw.Flush();
                        sw.Close();
                    }
                }
            }

            byte[] data = File.ReadAllBytes(pathToTemp + "app.zip");

            Directory.Delete(pathToTemp, true);

            return data;
        }
    }
}
