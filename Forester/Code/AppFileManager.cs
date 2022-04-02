using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester
{
    public static class AppFileManager
    {
        public static string PrepareApp(string pathToFolder)
        {
            Clear();

            ZipFile.CreateFromDirectory(pathToFolder, "./tempApp.zip");
            return "./tempApp.zip";
        }

        public static void AppDownloaded(string appName)
        {
            string currentZipLocation = $"./Apps/{appName}/";

            Directory.CreateDirectory(currentZipLocation);
            File.Move("./tempApp.zip", currentZipLocation + "tempApp.zip", true);
            ZipFile.ExtractToDirectory(currentZipLocation + "tempApp.zip", currentZipLocation, true);
            File.Delete(currentZipLocation + "tempApp.zip");
        }

        public static void AppDelete(string appName)
        {
            string currentZipLocation = $"./Apps/{appName}/";

            if(Directory.Exists(currentZipLocation))
                Directory.Delete(currentZipLocation, true);
        }

        public static Dictionary<string, string> ReadAppConfig(string appName)
        {
            if(File.Exists($"./Apps/{appName}/ForesterConfig/config.json"))
                return JsonConverter.Deserialize<Dictionary<string, string>>(FileReader.ReadText($"./Apps/{appName}/ForesterConfig/config.json"));

            return new Dictionary<string, string>();
        }

        public static void Clear()
        {
            if (File.Exists("./tempApp.zip"))
                File.Delete("./tempApp.zip");
        }
    }
}
