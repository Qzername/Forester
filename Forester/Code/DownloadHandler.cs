using Forester.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace Forester
{
    public static class DownloadHandler
    {
        static readonly string appsFolder = "./Apps/";
        static Application app;

        public static bool IsDownloaded(string name)
        {
            if (Directory.Exists(appsFolder + name))
                return true;

            return false;
        }

        public static Application GetInfo(string name)
        {
            StreamReader sr = File.OpenText(appsFolder + name + "/" +name );
            string json = sr.ReadToEnd();
            sr.Close();

            var app = JsonConvert.DeserializeObject<Application>(json);
            app.isInDownloadFolder = Directory.GetFiles(appsFolder + app.name).Length == 0 ? "false" : "true";

            return app;
        }

        public static void Download(Application app)
        {
            if(File.Exists(appsFolder + app.name + "/" + app.name))
            {
                string[] files = Directory.GetFiles(appsFolder + app.name + "/");
                foreach (string file in files)
                    File.Delete(file);
            }
            Directory.CreateDirectory(appsFolder + app.name);

            using (WebClient wc = new WebClient())
            {
                DownloadHandler.app = app;
                wc.DownloadFile(
                    new Uri($"http://localhost:5000/api/Download/Load/{Data.currentAccount.id}/{Data.currentAccount.password}/{app.name}"),
                     appsFolder+ app.name +".zip"
                );
            }
            Directory.CreateDirectory(appsFolder + app.name);
            ZipFile.ExtractToDirectory(appsFolder + app.name + ".zip", appsFolder + app.name);

            File.Create(appsFolder + app.name + "/" + app.name).Close();
            File.WriteAllText(appsFolder + app.name + "/" + app.name, JsonConvert.SerializeObject(app));
            File.Delete(appsFolder + app.name + ".zip");
        }

    }
}
