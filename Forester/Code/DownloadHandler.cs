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
        
        /// <summary>
        /// Checking if program is downloaded
        /// </summary>
        /// <param name="name">Name of application</param>
        public static bool IsDownloaded(string name)
        {
            if (Directory.Exists(appsFolder + name))
                return true;

            return false;
        }

        /// <summary>
        /// Getting info json from downloaded file
        /// </summary>
        /// <param name="name">Name of application</param>
        public static Application GetInfo(string name)
        {
            //Opening file and getting info from that file
            StreamReader sr = File.OpenText(appsFolder + name + "/" +name );
            string json = sr.ReadToEnd();
            sr.Close();

            //Converting it into Application object 
            var app = JsonConvert.DeserializeObject<Application>(json);

            return app;
        }

        /// <summary>
        /// Downloading the app into 
        /// </summary>
        /// <param name="app"></param>
        public static async Task Download(Application app)
        {
            //Deleting older version of program (if exists)
            if(File.Exists(appsFolder + app.name + "/" + app.name))
            {
                string[] files = Directory.GetFiles(appsFolder + app.name + "/");
                foreach (string file in files)
                    File.Delete(file);
            }

            //Creating app directory
            Directory.CreateDirectory(appsFolder + app.name);

            //Downloading .zip with app (TODO: async download)
            WebClient wc = new WebClient();
            await wc.DownloadFileTaskAsync(new Uri(ServerConnection.api + $"/api/Download/Load/{Data.currentAccount.id}/{Data.currentAccount.password}/{app.name}"),
                    appsFolder+ app.name +".zip");

            await Task.Delay(1000);

            //Creating info .json, extracting program, cleaning up
            Directory.CreateDirectory(appsFolder + app.name);
            ZipFile.ExtractToDirectory(appsFolder + app.name + ".zip", appsFolder + app.name);
            File.Create(appsFolder + app.name + "/" + app.name).Close();
            File.WriteAllText(appsFolder + app.name + "/" + app.name, JsonConvert.SerializeObject(app));
            File.Delete(appsFolder + app.name + ".zip");
        }

        /// <summary>
        /// Uploading app
        /// </summary>
        /// <param name="app">info about app</param>
        /// <param name="pathToDir">directory to app</param>
        public static async Task Upload(Application app, string pathToDir)
        {
            File.Create(pathToDir + "/" + app.name).Close(); 
            File.WriteAllText(pathToDir + "/" + app.name, JsonConvert.SerializeObject(app));

            if (File.Exists("./upload.zip"))
                File.Delete("./upload.zip");

            ZipFile.CreateFromDirectory(pathToDir, "./upload.zip");

            File.Delete(pathToDir + "/" + app.name);

            WebClient wc = new WebClient();
            await wc.UploadFileTaskAsync(new Uri(ServerConnection.api + $"/api/Download/Upload/{app.author.id}/{app.author.password}/{app.name}"),"POST", "./upload.zip");
        }
    }
}