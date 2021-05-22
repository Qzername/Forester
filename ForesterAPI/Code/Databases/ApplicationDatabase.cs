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
        public static readonly string baseApplication = "./Database/Applications/";
        public static readonly string baseDownload = "./Database/Download/";

        public static bool DoesExist(string name)
        {
            if (File.Exists(baseApplication + name))
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
                sr.Close();

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
            if (string.IsNullOrEmpty(app.version))
                app.version = "1.0v";

            if(!(app.allowedAccounts is null))
            {
                Account[] allowedAccounts = new Account[app.allowedAccounts.Length];

                for(int i = 0; i < allowedAccounts.Length;i++)
                    allowedAccounts[i] = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE id={app.allowedAccounts[i].id}")[0];
            
                app.allowedAccounts = allowedAccounts;
            }

            string json = JsonConvert.SerializeObject(app);

            using (StreamWriter sw = File.CreateText(baseApplication + app.name))
                foreach (string line in json.Split(new[] { '\r', '\n' }))
                    sw.WriteLine(line);

            Directory.CreateDirectory(baseDownload + app.name);
        }

        public static void Update(Application app)
        {
            if (string.IsNullOrEmpty(app.version))
                app.version = "1.0v";

            if (!(app.allowedAccounts is null))
            {
                Account[] allowedAccounts = new Account[app.allowedAccounts.Length];

                for (int i = 0; i < allowedAccounts.Length; i++)
                    allowedAccounts[i] = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE id={app.allowedAccounts[i].id}")[0];

                app.allowedAccounts = allowedAccounts;
            }

            File.WriteAllText(baseApplication + app.name, string.Empty);
            File.WriteAllText(baseApplication + app.name, JsonConvert.SerializeObject(app));
        }

        public static Application RemovePasswords(Application app)
        {
            var author = app.author;
            author.password = string.Empty;
            app.author = author;

            for(int i =0; i<app.allowedAccounts.Length;i++)
            {
                var user = app.allowedAccounts[i];
                user.password = string.Empty;
                app.allowedAccounts[i] = user;
            }

            return app;
        }
    }
}
