using ForesterAPI.Databases;
using ForesterAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Threading.Tasks;

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

            if (users.Length == 0 || users[0].isDeveloper == "false")
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

        [HttpPost("[action]/{id}/{password}/{name}")]
        public async void Upload(int id, string password, string name, IFormFile file)
        {
            var users = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE id={id} AND password=\"{password}\"");

            if (file.Length < 0 || users.Length == 0 || users[0].isDeveloper == "false")
                return;

            var stream = new FileStream("./Database/" + name + ".zip", FileMode.Create);
            await file.CopyToAsync(stream);
            stream.Close();

            if (!ApplicationDatabase.DoesExist(name))
                return;

            if (ApplicationDatabase.Get().Single(x => x.name == name).isInDownloadFolder == "true")
            {
                string[] files = Directory.GetFiles(ApplicationDatabase.baseDownload + $"{name}/");
                foreach (string fileS in files)
                    System.IO.File.Delete(fileS);
            }

            ZipFile.ExtractToDirectory("./Database/" + name + ".zip", ApplicationDatabase.baseDownload + "/" + name + "/");

            System.IO.File.Delete(ApplicationDatabase.baseApplication + $"/{name}");
            System.IO.File.Move(ApplicationDatabase.baseDownload + $"/{name}/{name}", ApplicationDatabase.baseApplication + $"/{name}");
            System.IO.File.Move("./Database/" + name + ".zip", ApplicationDatabase.baseDownload + name +"/"+ name +".zip");
        }
    }
}