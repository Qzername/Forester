using ForesterAPI.Databases;
using ForesterAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ForesterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Applications : ControllerBase
    {
        [HttpGet]
        public string testValue()
        {
            VerifiedApplication Vapp = new VerifiedApplication();
            Vapp.application = new Application()
            {
                name = "SpamBox 2.0",
                author = new Account()
                {
                    id = 2,
                    name = "uzer",
                    password = "ilovyou",
                    isDeveloper = "true"
                },
                allowedAccounts = new Account[]
                {
                    new Account()
                    {
                        id = 2,
                        name = "uzer",
                        password = "ilovyou",
                        isDeveloper = "true"
                    }
                },
                isPrivate = "false"
            };
            Vapp.verificationKey = new VerificationKey()
            {
                id = 2,
                password = "ilovyou"
            };

            return JsonConvert.SerializeObject(Vapp);
        }

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
                    PublishedApps.Add(app);

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
                if(app.allowedAccounts is not null)
                    foreach (Account acc in app.allowedAccounts)
                        if (acc.id == users[0].id && acc.password == users[0].password && app.isPrivate == "true")
                            PrivateApps.Add(app);

            return JsonConvert.SerializeObject(PrivateApps.ToArray());
        }

        [HttpGet("[action]")]
        public string GetPublic()
        {
            Application[] table = ApplicationDatabase.Get();
            List<Application> final = new List<Application>();

            foreach (Application app in table)
                if (app.isPrivate == "false")
                    final.Add(app);

            return JsonConvert.SerializeObject(final.ToArray());
        }

        [HttpPost("[action]")]
        public string PublishNew([FromBody] VerifiedApplication value)
        {
            var users = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE id={value.verificationKey.id} AND password=\"{value.verificationKey.password}\"");

            if (users.Length == 0)
                return "USER NOT VERIFIED.";

           // if (users[0].isDeveloper != "true")
           //     return "USER NOT PERMITED";

            if (!ApplicationDatabase.DoesExist(value.application.name))
                ApplicationDatabase.CreateNew(value.application);
            else
                return "APPLICATION ALREADY EXIST.";

            return "OK.";
        }
    }
}
