using ForesterAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace ForesterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        #region Forester
        [HttpGet("Forester/Version")]
        public IActionResult VersionForester() => Ok(System.IO.File.ReadAllText("./Database/Update/Forester/version.json"));

        [HttpPost("Forester/Download")]
        public FileContentResult DownloadForester([FromBody] Dictionary<string, string> files) 
        {
            //Zawiera wszystkie pliki wraz z ich wielkością
            var config = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText($"./Database/Update/Forester/config.json"));

            List<string> filesToRemove = new List<string>();

            //usunięcie plików bez zmian, oznaczenie niepotrzebnych jako do usuniecia
            foreach (KeyValuePair<string, string> key in files)
                if (config.ContainsKey(key.Key) && config[key.Key] == key.Value)
                    filesToRemove.Add(key.Key);
                else
                    config[key.Key] = "REMOVE";

            //Skopiowanie zipa do tempa, usunięcie plików, przesłanie dalej
            string pathToTemp = $"./Database/Temp/{new Random().Next(0, 100000)}/";
            string pathToAppZip = $"./Database/Update/Forester/forester.zip";

            Directory.CreateDirectory(pathToTemp);
            System.IO.File.Copy(pathToAppZip, pathToTemp + "app.zip");

            using (ZipArchive archive = ZipFile.Open(pathToTemp + "app.zip", ZipArchiveMode.Update))
            {
                var entriesToRemove = archive.Entries.Where(x => filesToRemove.Contains(x.FullName)).ToList();

                for (int i = 0; i < entriesToRemove.Count; i++)
                    entriesToRemove[i].Delete();

                var entry = archive.CreateEntry("config.json");
                using (Stream stream = entry.Open())
                {
                    var sw = new StreamWriter(stream);
                    sw.Write(JsonConvert.SerializeObject(config));
                    sw.Flush();
                    sw.Close();
                }
            }

            byte[] data = System.IO.File.ReadAllBytes(pathToTemp + "app.zip");

            Directory.Delete(pathToTemp, true);

            return File(data, "application/force-download", "forester.zip"); 
        }

        [HttpPost("Forester/UploadNewVersion")]
        [DisableRequestSizeLimit]
        public IActionResult UploadNewVersionForester([FromHeader] string token, [FromQuery] string version, IFormFile file)
        {
            //Folder -> ./Database/Update/Forester/

            if (token != "dfsfgopsd jgoeiow iodfw f0wef ihjegfr8d;g uesf iesdoifjcsdfc oerhgcv oeuic awgf78w6fepbgj krhe uef ewkjfj eru gfux nevge ruygzayurbvz,oerbtvyzetyrvbtezmbuvi")
                return StatusCode(403);

            if (System.IO.File.Exists("./Database/Update/Forester/forester.zip"))
                System.IO.File.Delete("./Database/Update/Forester/forester.zip");

            FileStream stream = new FileStream("./Database/Update/Forester/forester.zip", FileMode.Create);
            file.CopyTo(stream);
            stream.Close();

            ZipArchive zip = ZipFile.Open("./Database/Update/Forester/forester.zip", ZipArchiveMode.Read);

            Dictionary<string, string> json = new Dictionary<string, string>();

            foreach (var entry in zip.Entries)
            {
                string hash;

                var md5 = MD5.Create();

                var streamAnother = entry.Open();

                var hashMD5 = md5.ComputeHash(streamAnother);

                hash = BitConverter.ToString(hashMD5).Replace("-", "").ToLowerInvariant();

                streamAnother.Close();

                json.Add(entry.FullName, hash);
            }

            zip.Dispose();

            System.IO.File.WriteAllText("./Database/Update/Forester/config.json", JSONManager.Serialize(json));
            System.IO.File.WriteAllText("./Database/Update/Forester/version.json", "{ \"version\":\"" + version + "\"}");

            return Ok();
        }
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
        public IActionResult UpdatePicture([FromHeader] string token, [FromQuery] PictureManager.Folder objectType, [FromQuery] PictureManager.Picture pictureType, [FromQuery] string name, IFormFile file)
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
