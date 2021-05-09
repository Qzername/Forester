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
                foreach (Account acc in app.allowedAccounts)
                    if (acc.id == id && acc.password == password)
                        PrivateApps.Add(app);

            return JsonConvert.SerializeObject(PrivateApps.ToArray());
        }

        [HttpGet("[action]")]
        public string GetPublic() => JsonConvert.SerializeObject(ApplicationDatabase.Get());

        [HttpPost("[action]")]
        public string PublishNew([FromBody] VerifiedApplication value)
        {
            var users = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE id={value.verificationKey.id} AND password=\"{value.verificationKey.password}\"");

            if (users.Length == 0)
                return "USER NOT VERIFIED.";

            if (!ApplicationDatabase.DoesExist(value.application.name))
                ApplicationDatabase.CreateNew(value.application);
            else
                return "APPLICATION ALREADY EXIST.";

            return "OK.";
        }
    }
}
