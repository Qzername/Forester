using ForesterAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForesterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        // GET api/<DownloadController>
        [HttpGet("[action]")]
        public FileContentResult Load(Token token, string name, Dictionary<string, string> files)
        {
            string decoded = JWTManager.Decode(token.token);

            if (decoded == "")
                return null;

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);

            var apps = SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE name=\"{name}\"");

            if (apps.Length == 0)
                return null;

            var app = apps[0];

            if (app.isPrivate == "True" && app.mainDeveloper != loginToken.ID && SQLDatabase.Select<Application>($"SELECT Applications.* FROM Applications, Developers WHERE Applications.ID = {app.ID} AND Developers.ID_User = {loginToken.ID}").Length == 0)
                return null;

            if (!ApplicationManager.CheckIfExist(app.name))
                return null;

            if (app.absoluteUpdate == "True")
                files = new Dictionary<string, string>();

            return File(ApplicationManager.GetFiles(name, files), "application/force-download", app.name + ".zip");
        }

        // POST api/<DownloadController>
        [HttpPost("[action]")]
        public IActionResult Upload(Token token, string name, IFormFile file)
        {
            string decoded = JWTManager.Decode(token.token);

            if (decoded == "")
                return StatusCode(403);

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);

            var user = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE ID = {loginToken.ID}")[0];

            if (!user.isDeveloper || file.Length != 1)
                return StatusCode(403);

            ApplicationManager.SaveAndCreateConfig(name, file);

            return Ok();
        }
    }

}