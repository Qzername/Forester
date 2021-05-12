using ForesterAPI.Databases;
using ForesterAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

namespace ForesterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Download : ControllerBase
    {
        // GET: api/<Download>
        [HttpGet("[action]/{id}/{password}/{name}")]
        public FileContentResult Load(int id, string password, string name)
        {
            var users = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE id={id} AND password=\"{password}\"");

            if (users.Length == 0)
                return null;

            var apps = ApplicationDatabase.Get();

            if (!apps.Any(x => x.name == name))
                return null;

            Application app = apps.Single(x => x.name == name);

           // if (app.isInDownloadFolder == "false")
           //     return null;
            
            byte[] fileBytes = System.IO.File.ReadAllBytes("./Database/Download/"+app.name+"/"+ app.name + ".zip");

            return File(fileBytes, "application/force-download", app.name + ".zip");
        }
    }
}
