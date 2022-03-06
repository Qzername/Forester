using ForesterAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForesterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        // POST api/<DownloadController>
        [HttpPost("[action]")]
        public ActionResult Load([FromHeader] string token, [FromQuery] string name, [FromBody] Dictionary<string, string> files)
        {
            string decoded = JWTManager.Decode(token);

            if (decoded == "")
                return StatusCode(403);

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);

            var apps = SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE name=\"{name}\"");

            if (apps.Length == 0)
                return StatusCode(404);

            var app = apps[0];

            if (app.isPrivate == "True" && app.mainDeveloper != loginToken.ID && SQLDatabase.Select<Application>($"SELECT Applications.* FROM Applications, Developers WHERE Applications.ID = {app.ID} AND Developers.ID_User = {loginToken.ID}").Length == 0)
                return StatusCode(404);

            if (!ApplicationManager.CheckIfExist(app.name))
                return StatusCode(404);

            return File(ApplicationManager.GetFiles(name, files), "application/force-download", app.name + ".zip");
        }

        // POST api/<DownloadController>
        [HttpPost("[action]")]
        public IActionResult Upload([FromHeader] string token, [FromQuery] string name, IFormFile file)
        {
            string decoded = JWTManager.Decode(token);

            if (decoded == "")
                return StatusCode(401);

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);

            var app = SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE name = \"{name}\"");

            if (app.Length == 0)
                return StatusCode(403);

            if (app[0].mainDeveloper != loginToken.ID && SQLDatabase.Select<ulong>($"SELECT ID_User FROM Developers WHERE ID_Application = \"{app[0].ID}\" AND ID_User =\"{loginToken.ID}\"").Length > 0)
                return StatusCode(404);

            ApplicationManager.SaveAndCreateConfig(name, file);

            return Ok();
        }
    }

}