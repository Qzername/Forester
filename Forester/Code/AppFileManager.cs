using Forester.Models;
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

        public static void AppDownloaded(AppConfig appConfig, string appName)
        {
            string currentZipLocation;

            if (appConfig.isDefaultPath)
                currentZipLocation = $"./Apps/{appName}/";
            else
                currentZipLocation = appConfig.path + "/";

            //remove not necessary files
            if(appConfig.deleteNotNecessaryFiles)
            {
                var config = ReadAppConfig(appConfig, appName);
            
                foreach(var element in config.Where(x=>x.Value == "REMOVE"))
                    File.Delete(currentZipLocation + element.Key);
            }

            Directory.CreateDirectory(currentZipLocation);
            File.Move("./tempApp.zip", currentZipLocation + "tempApp.zip", true);
            ZipFile.ExtractToDirectory(currentZipLocation + "tempApp.zip", currentZipLocation, true);
            File.Delete(currentZipLocation + "tempApp.zip");
        }

        public static void AppDelete(AppConfig appConfig, string appName)
        {
            string currentZipLocation;

            if (appConfig.isDefaultPath)
                currentZipLocation = $"./Apps/{appName}/";
            else
                currentZipLocation = appConfig.path + "/";

            if (!Directory.Exists(currentZipLocation))
                return;

            if (appConfig.isDefaultPath)
                Directory.Delete(currentZipLocation, true);
            else
            {
                var config = ReadAppConfig(appConfig, appName);

                foreach(var item in config)
                {
                    if (!appConfig.deleteNotNecessaryFiles && item.Value == "REMOVE")
                        continue;
                    
                    File.Delete(currentZipLocation + item.Key);
                }

                Directory.Delete(currentZipLocation + "ForesterConfig/",true);
            }
        }

        public static bool IsAppDownloaded(AppConfig appConfig, string appName)
        {
            //older versions
            if (appConfig.path == "null")
                return false;

            return (!appConfig.isDefaultPath && File.Exists(appConfig.path + "/ForesterConfig/config.json") || appConfig.isDefaultPath && File.Exists($"./Apps/{appName}/ForesterConfig/config.json"));
        }

        public static Dictionary<string, string> ReadAppConfig(AppConfig appConfig, string appName)
        {
            if (!appConfig.isDefaultPath && File.Exists(appConfig.path + "/ForesterConfig/config.json"))
                return JsonConverter.Deserialize<Dictionary<string, string>>(FileReader.ReadText(appConfig.path + "/ForesterConfig/config.json"));
            else if(appConfig.isDefaultPath && File.Exists($"./Apps/{appName}/ForesterConfig/config.json"))
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
