using Newtonsoft.Json;
using System.IO.Compression;
using System.Security.Cryptography;

namespace ForesterAPI
{
    public static class ApplicationManager
    {
        public static bool CheckIfExist(string name) => File.Exists($"./Database/Applications/{name}/config.json");
        public static void Rename(string oldName, string newName) => Directory.Move("./Database/Applications/" + oldName, "./Database/Applications/" + newName);
        
        public static void SaveAndCreateConfig(string name, IFormFile file)
        {
            Clear(name);

            if (!Directory.Exists("./Database/Applications/" + name + "/"))
                Directory.CreateDirectory("./Database/Applications/" + name + "/");

            using (FileStream stream = new FileStream($"./Database/Applications/{name}/app.zip", FileMode.Create))
                file.CopyTo(stream);

            ZipArchive zip = ZipFile.Open($"./Database/Applications/{name}/app.zip", ZipArchiveMode.Read);
            
            Dictionary<string, string> json = new Dictionary<string, string>();

            foreach (var entry in zip.Entries)
            {
                string hash;

                using (var md5 = MD5.Create())
                {
                    using (var stream = entry.Open())
                    {
                        var hashMD5 = md5.ComputeHash(stream);

                        hash = BitConverter.ToString(hashMD5).Replace("-", "").ToLowerInvariant();
                    }
                }

                json.Add(entry.FullName, hash);
            }

            File.WriteAllText($"./Database/Applications/{name}/config.json", DicToJson(json));
        }

        public static byte[] GetFiles(string name, Dictionary<string, string> alreadyExist)
        {
            //Zawiera wszystkie pliki wraz z ich wielkością
            var config = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($"./Database/Applications/{name}/config.json"));

            List<string> filesToRemove= new List<string>();

            //usunięcie plików bez zmian, oznaczenie niepotrzebnych jako do usuniecia
            foreach (KeyValuePair<string, string> key in alreadyExist)
                if (config.ContainsKey(key.Key))
                {
                    if (config[key.Key] != key.Value || config[key.Key].StartsWith("ForesterConfig"))
                        filesToRemove.Add(key.Key);
                }
                else
                    config[key.Key] = "REMOVE";

            //Skopiowanie zipa do tempa, usunięcie plików, przesłanie dalej
            string pathToTemp = $"./Database/Temp/{new Random().Next(0, 100000)}/";
            string pathToAppZip = $"./Database/Applications/{name}/app.zip";

            Directory.CreateDirectory(pathToTemp);
            File.Copy(pathToAppZip, pathToTemp + "app.zip");

            using (ZipArchive archive = ZipFile.Open(pathToTemp + "app.zip", ZipArchiveMode.Update))
            {
                foreach (var item in archive.Entries)
                    if (filesToRemove.Contains(item.Name))
                        item.Delete();
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
