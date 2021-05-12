using ForesterAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ForesterAPI.Databases
{
    public static class ApplicationDatabase
    {
        static readonly string baseApplication = "./Database/Applications/";
        static readonly string baseDownload = "./Database/Download/";

        public static bool DoesExist(string name)
        {
            if (Directory.Exists(baseApplication + name))
                return true;

            return false;
        }

        public static Application[] Get()
        {
            var applications = Directory.GetFiles(baseApplication);
            List<Application> FinalApplications = new List<Application>();

            foreach(string fileName in applications)
            {
                string json;

                StreamReader sr = File.OpenText(fileName);
                json = sr.ReadToEnd();

                var app = JsonConvert.DeserializeObject<Application>(json);
                app.isInDownloadFolder = Directory.GetFiles(baseDownload + app.name).Length == 0 ? "false" : "true";

                FinalApplications.Add(app);
            }

            return FinalApplications.ToArray();
        }

        /*
        public static Application Get(string name)
        {
            string json;

            StreamReader sr = File.OpenText(baseApplication + name);
            json = sr.ReadToEnd();

            var application = (Application)JsonConvert.DeserializeObject(json);

            application.isInDownloadFolder = Directory.GetFiles(baseDownload + application.name).Length == 0 ? "false" : "true";

            return application;
        }*/

        public static void CreateNew(Application app)
        {
            string json = JsonConvert.SerializeObject(app);

            if (string.IsNullOrEmpty(app.version))
                app.version = "1.0v";

            using (StreamWriter sw = File.CreateText(baseApplication + app.name))
                foreach (string line in json.Split(new[] { '\r', '\n' }))
                    sw.WriteLine(line);

            Directory.CreateDirectory(baseDownload + app.name);
        }
    }
}
