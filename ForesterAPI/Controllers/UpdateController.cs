using ForesterAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ForesterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        #region Forester
        [HttpGet("Forester/Version")]
        public IActionResult VersionForester() => Ok(System.IO.File.ReadAllText("./Database/Update/Forester/version.json"));

        [HttpGet("Forester/Download")]
        public FileContentResult DownloadForester() => File(System.IO.File.ReadAllBytes("./Database/Update/Forester/app.zip"), "application/force-download", "forester.zip");
        #endregion

        #region Updater
        [HttpGet("Updater/Version")]
        public IActionResult VersionUpdater() => Ok(System.IO.File.ReadAllText("./Database/Update/Updater/version.json"));

        [HttpGet("Updater/Download")]
        public FileContentResult DownloadUpdater() => File(System.IO.File.ReadAllBytes("./Database/Update/Updater/app.zip"), "application/force-download", "updater.zip");
        #endregion

        #region Picture
        [HttpGet("[action]")]
        public ActionResult GetPicture([FromHeader] string token, [FromQuery] PictureManager.Folder objectType, [FromQuery] PictureManager.Picture pictureType, [FromQuery] string name)
        {
            string folder = string.Empty;

            string decoded = JWTManager.Decode(token);

            if (decoded == "")
                return StatusCode(403);

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);

            if (objectType == PictureManager.Folder.Accounts)
            {
                string[] splitedName = name.Split('#');

                if (splitedName.Length < 2)
                    return StatusCode(403);

                string friendly_name = string.Join("", splitedName.Take(splitedName.Length - 1));
                string code = splitedName[splitedName.Length - 1];

                if (code.Length != 4)
                    return StatusCode(403);

                var account = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE friendly_ID={code} AND friendly_username = \"{friendly_name}\"");

                if (code.Length == 0)
                    return StatusCode(403);

                name = account[0].ID.ToString();
                folder = name;
            }

            (bool profilePicture, bool backgroundPicture) = PictureManager.CheckIfExist(name, objectType);

            if ((pictureType == PictureManager.Picture.profile && !profilePicture) || (pictureType == PictureManager.Picture.background && !backgroundPicture))
                return StatusCode(402);

            if(objectType == PictureManager.Folder.Applications)
            {
                var apps = SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE name=\"{name}\"");

                if (apps.Length == 0)
                    return StatusCode(404);

                var app = apps[0];
                folder = app.name;

                if (app.isPrivate == "True" && loginToken.ID != app.mainDeveloper && SQLDatabase.Select<Application>($"SELECT Applications.* FROM Applications, Developers WHERE Applications.ID = {app.ID} AND Developers.ID_User = {loginToken.ID}").Length == 0)
                    return StatusCode(404);
            }

            return File(PictureManager.GetImage(folder, pictureType, objectType), "image/png");
        }

        [HttpPut("[action]")]
        public IActionResult UpdatePicture([FromHeader] string token, PictureManager.Folder objectType, PictureManager.Picture pictureType, string name, IFormFile file)
        {
            string decoded = JWTManager.Decode(token);

            if (decoded == "")
                return StatusCode(403);

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);

            if (objectType == PictureManager.Folder.Applications && SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE mainDeveloper = {loginToken.ID}").Length == 0)
                return StatusCode(404);

            if (objectType == PictureManager.Folder.Accounts && loginToken.username != name)
                return StatusCode(403);

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();

                PictureManager.UpdateImage(objectType == PictureManager.Folder.Accounts? loginToken.ID.ToString() : name, pictureType, objectType, fileBytes);
            }

            return Ok();
        }
        #endregion
    }
}
