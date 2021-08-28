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
using Path = Forester.Models.Path;
using System.Diagnostics;

namespace Forester
{
    public static class DownloadHandler
    {
        public static readonly string appsFolder = "./Apps/";
        
        /// <summary>
        /// Checking if program is downloaded
        /// </summary>
        /// <param name="name">Name of application</param>
        public static bool IsDownloaded(string name, string path = "")
        {
            string directoryPath;

            if (path == "")
                directoryPath = appsFolder;
            else
                directoryPath = path;

            if (Directory.Exists(directoryPath + name) && File.Exists(directoryPath + name + "/" + name) )
                return true;
            else if(Directory.Exists(directoryPath + name) && !File.Exists(directoryPath + name + "/path.json"))
                Directory.Delete(directoryPath + name, true);

            if (Directory.Exists(appsFolder + name))
                Directory.Delete(appsFolder + name, true);

            return false;
        }

        /// <summary>
        /// Getting info json from downloaded file
        /// </summary>
        /// <param name="name">Name of application</param>
        public static Application GetInfo(string name, string path = "")
        {
            //Opening file and getting info from that file

            StreamReader sr = File.OpenText(path == "" ? appsFolder + name + "/" +name : path + name + "/" + name);
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
        public static async Task<bool> Download(Application app, string path = "")
        {
            string directoryPath;

            if (path == "")
                directoryPath = appsFolder;
            else
            {
                directoryPath = path + "/";

                if (!Directory.Exists(appsFolder + app.name))
                    Directory.CreateDirectory(appsFolder + app.name);

                var stream = File.CreateText(appsFolder + app.name + "/path.json");
                stream.WriteLine(JsonConvert.SerializeObject(new Path() { path = directoryPath }));
                stream.Close();
            }

            //Deleting older version of program (if exists)
            if (File.Exists(directoryPath + app.name + "/" + app.name))
            {
                DirectoryInfo di = new DirectoryInfo(appsFolder + app.name + "/");

                foreach (FileInfo fileS in di.GetFiles())
                    fileS.Delete();
                foreach (DirectoryInfo dir in di.GetDirectories())
                    dir.Delete(true);
            }

            //Creating app directory
            Directory.CreateDirectory(path == "" ? directoryPath +app.name : directoryPath);

           try
           {
                //Downloading .zip with app (TODO: async download)
                WebClient wc = new WebClient();
                await wc.DownloadFileTaskAsync(new Uri(ServerConnection.api + $"/api/Download/Load/{Data.currentAccount.id}/{Data.currentAccount.password}/{app.name}"),
                        appsFolder + app.name + ".zip");

                await Task.Delay(1000);

                //Creating info .json, extracting program, cleaning up
                Directory.CreateDirectory(directoryPath + app.name);
                ZipFile.ExtractToDirectory(appsFolder + app.name + ".zip", directoryPath + app.name);
                File.Create(directoryPath + app.name + "/" + app.name).Close();
                File.WriteAllText(directoryPath + app.name + "/" + app.name, JsonConvert.SerializeObject(app));
                File.Delete(appsFolder + app.name + ".zip");
                return true;
            }
            catch(Exception x)
            {
                Debug.Print(x.Message);
                return false;
            }
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