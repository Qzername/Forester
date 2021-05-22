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

        [HttpPost("[action]/{id}/{password}/{name}")]
        public async Task Upload(int id, string password, string name, IFormFile file)
        {
            var users = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE id={id} AND password=\"{password}\"");

            if (file.Length < 0 || users.Length == 0 || users[0].isDeveloper == "false")
                return;

            using (FileStream stream = new FileStream("./Database/" + name + ".zip", FileMode.Create)) 
                await file.CopyToAsync(stream);

            if (!ApplicationDatabase.DoesExist(name))
                return;

            if (ApplicationDatabase.Get().Single(x => x.name == name).isInDownloadFolder == "true")
            {
                DirectoryInfo di = new DirectoryInfo(ApplicationDatabase.baseDownload + $"{name}/");

                foreach (FileInfo fileS in di.GetFiles())
                    fileS.Delete();
                foreach (DirectoryInfo dir in di.GetDirectories())
                    dir.Delete(true);
            }

            ZipFile.ExtractToDirectory("./Database/" + name + ".zip", ApplicationDatabase.baseDownload + "/" + name + "/");

            System.IO.File.Delete(ApplicationDatabase.baseApplication + $"/{name}");
            System.IO.File.Move(ApplicationDatabase.baseDownload + $"/{name}/{name}", ApplicationDatabase.baseApplication + $"/{name}");
            System.IO.File.Move("./Database/" + name + ".zip", ApplicationDatabase.baseDownload + name +"/"+ name +".zip");
        }
    }
}