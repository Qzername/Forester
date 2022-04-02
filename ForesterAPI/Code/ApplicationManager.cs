using Newtonsoft.Json;
using System.IO.Compression;
using System.Security.Cryptography;

namespace ForesterAPI
{
    public static class ApplicationManager
    {
        public static bool CheckIfExist(string name) => File.Exists($"./Database/Applications/{name}/config.json");
        public static void Rename(string oldName, string newName)
        {
            if (Directory.Exists("./Database/Applications/" + oldName))
                Directory.Move("./Database/Applications/" + oldName, "./Database/Applications/" + newName);

            if(Directory.Exists("./Database/Pictures/Applications/" + oldName))
                Directory.Move("./Database/Pictures/Applications/" + oldName, "./Database/Pictures/Applications/" + newName);
        }
        public static void SaveAndCreateConfig(string name, IFormFile file)
        {
            Clear(name);

            if (!Directory.Exists("./Database/Applications/" + name + "/"))
                Directory.CreateDirectory("./Database/Applications/" + name + "/");

            FileStream stream = new FileStream($"./Database/Applications/{name}/app.zip", FileMode.Create);
            file.CopyTo(stream);
            stream.Close();

            ZipArchive zip = ZipFile.Open($"./Database/Applications/{name}/app.zip", ZipArchiveMode.Read);
            
            Dictionary<string, string> json = new Dictionary<string, string>();

            foreach (var entry in zip.Entries)
            {
                string hash;

                var md5 = MD5.Create();

                var streamAnother = entry.Open();

                var hashMD5 = md5.ComputeHash(streamAnother);

                hash = BitConverter.ToString(hashMD5).Replace("-", "").ToLowerInvariant();

                streamAnother.Close();

                json.Add(entry.FullName, hash);
            }

            zip.Dispose();

            File.WriteAllText($"./Database/Applications/{name}/config.json", DicToJson(json));
        }

        public static byte[] GetFiles(string name, Dictionary<string, string> alreadyExist)
        {
            //Zawiera wszystkie pliki wraz z ich wielkością
            var config = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($"./Database/Applications/{name}/config.json"));

            List<string> filesToRemove= new List<string>();

            //usunięcie plików bez zmian, oznaczenie niepotrzebnych jako do usuniecia
            foreach (KeyValuePair<string, string> key in alreadyExist)
                if (config.ContainsKey(key.Key) && config[key.Key] == key.Value)
                    filesToRemove.Add(key.Key);
                else
                    config[key.Key] = "REMOVE";

            //Skopiowanie zipa do tempa, usunięcie plików, przesłanie dalej
            string pathToTemp = $"./Database/Temp/{new Random().Next(0, 100000)}/";
            string pathToAppZip = $"./Database/Applications/{name}/app.zip";

            Directory.CreateDirectory(pathToTemp);
            File.Copy(pathToAppZip, pathToTemp + "app.zip");

            using (ZipArchive archive = ZipFile.Open(pathToTemp + "app.zip", ZipArchiveMode.Update))
            {
                var entriesToRemove = archive.Entries.Where(x => filesToRemove.Contains(x.FullName)).ToList();

                for (int i = 0; i < entriesToRemove.Count; i++)
                    entriesToRemove[i].Delete();

                archive.CreateEntry("ForesterConfig/");
                var entry = archive.CreateEntry("ForesterConfig/config.json");
                using(Stream stream = entry.Open())
                {
                    var sw = new StreamWriter(stream);
                    sw.Write(JsonConvert.SerializeObject(config));
                    sw.Flush();
                    sw.Close();
                }
            }

            byte[] data = File.ReadAllBytes(pathToTemp + "app.zip");

            Directory.Delete(pathToTemp, true);

            SQLDatabase.NoReturnQuery($"UPDATE Applications SET downloadNumber = downloadNumber + 1 WHERE name=\"{name}\"");

            return data;
        }

        static string DicToJson(Dictionary<string, string> dict)
        {
            var entries = dict.Select(d => string.Format("\"{0}\": \"{1}\"", d.Key, d.Value));
            return "{" + string.Join(",", entries) + "}";
        }

        static void Clear(string name)
        {
            if(File.Exists($"./Database/Applications/{name}/config.json"))
                File.Delete($"./Database/Applications/{name}/config.json");
    
            if(File.Exists($"./Database/Applications/{name}/app.zip"))
                File.Delete($"./Database/Applications/{name}/app.zip");
        }
    }
}
