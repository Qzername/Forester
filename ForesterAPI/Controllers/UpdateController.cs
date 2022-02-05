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
        public FileContentResult GetPicture([FromHeader] string token, PictureManager.Folder objectType, PictureManager.Picture pictureType, string name)
        {
            string decoded = JWTManager.Decode(token);

            if (decoded == "")
                return null;

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);

            (bool profilePicture, bool backgroundPicture) = PictureManager.CheckIfExist(name, objectType);

            if ((pictureType == PictureManager.Picture.profile && !profilePicture) || (pictureType == PictureManager.Picture.background && !backgroundPicture))
                return null;

            if(objectType == PictureManager.Folder.Applications)
            {
                var apps = SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE name=\"{name}\"");

                if (apps.Length == 0)
                    return null;

                var app = apps[0];

                if (app.isPrivate == "True" && loginToken.ID != app.mainDeveloper && SQLDatabase.Select<Application>($"SELECT Applications.* FROM Applications, Developers WHERE Applications.ID = {app.ID} AND Developers.ID_User = {loginToken.ID}").Length == 0)
                    return null;
            }

            return File(PictureManager.GetImage(name, pictureType, objectType), "image/png");
        }

        [HttpPut("[action]")]
        public IActionResult UpdatePicture([FromHeader] string token, PictureManager.Folder objectType, PictureManager.Picture pictureType, string name, IFormFile file)
        {
            string decoded = JWTManager.Decode(token);

            if (decoded == "")
                return StatusCode(403);

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);

            if (objectType == PictureManager.Folder.Applications && SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE mainDeveloper = {loginToken.ID}").Length == 0)
                return StatusCode(403);

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();

                PictureManager.UpdateImage(loginToken.username, pictureType, objectType, fileBytes);
            }

            return Ok();
        }
        #endregion
    }
}
