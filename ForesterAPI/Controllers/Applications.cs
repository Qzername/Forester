using ForesterAPI.Databases;
using ForesterAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ForesterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Applications : ControllerBase
    {
        [HttpGet("[action]/{id}/{password}")]
        public string GetPublished(int id, string password)
        {
            var users = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE id={id} AND password=\"{password}\"");

            if (users.Length == 0)
                return "USER NOT VERIFIED.";

            var apps = ApplicationDatabase.Get();

            List<Application> PublishedApps = new List<Application>();

            foreach (Application app in apps)
                if (app.author.id == id && app.author.password == password)
                    PublishedApps.Add(ApplicationDatabase.RemovePasswords(app));

            return JsonConvert.SerializeObject(PublishedApps.ToArray());
        }

        [HttpGet("[action]/{id}/{password}")]
        public string GetPrivate(int id, string password)
        {
            var users = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE id={id} AND password=\"{password}\"");

            if (users.Length == 0)
                return "USER NOT VERIFIED.";

            var apps = ApplicationDatabase.Get();

            List<Application> PrivateApps = new List<Application>();

            foreach (Application app in apps)
                if(!(app.allowedAccounts is null))
                    foreach (Account acc in app.allowedAccounts)
                        if (acc.id == users[0].id && acc.password == users[0].password && app.isPrivate == "true")
                            PrivateApps.Add(ApplicationDatabase.RemovePasswords(app));

            return JsonConvert.SerializeObject(PrivateApps.ToArray());
        }

        [HttpGet("[action]")]
        public string GetPublic()
        {
            Application[] table = ApplicationDatabase.Get();
            List<Application> final = new List<Application>();

            foreach (Application app in table)
                if (app.isPrivate == "false")
                    final.Add(ApplicationDatabase.RemovePasswords(app));

            return JsonConvert.SerializeObject(final.ToArray());
        }

        [HttpPost("[action]")]
        public string PublishNew([FromBody] VerifiedApplication value)
        {
            var users = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE id={value.verificationKey.id} AND password=\"{value.verificationKey.password}\"");

            if (users.Length == 0)
                return "USER NOT VERIFIED.";

            if (users[0].isDeveloper != "true")
                return "USER NOT PERMITED";

            if (!ApplicationDatabase.DoesExist(value.application.name))
                ApplicationDatabase.CreateNew(value.application);
            else
                return "APPLICATION ALREADY EXIST.";

            return "OK.";
        }

        [HttpPost("[action]")]
        public string UpdateInfo([FromBody] VerifiedApplication value)
        {
            var users = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE id={value.verificationKey.id} AND password=\"{value.verificationKey.password}\"");

            if (users.Length == 0)
                return "USER NOT VERIFIED.";

            if (users[0].isDeveloper != "true")
                return "USER NOT PERMITED";

            if (ApplicationDatabase.DoesExist(value.application.name))
                ApplicationDatabase.Update(value.application);
            else
                return "APPLICATION DOESN'T EXISTS.";

            return "OK.";
        }
    }
}
