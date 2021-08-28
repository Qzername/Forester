using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ForesterAPI.Models;

namespace ForesterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Info : ControllerBase
    {
        [HttpGet("[action]")]
        public string GetVersion()
        {
            var application = JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText("./Database/Forester/config.json"));

            return application.version;
        }

        [HttpGet("[action]")]
        public FileContentResult Download()
        {
            string[] files = System.IO.Directory.GetFiles("./Database/Forester/");

            string rightPath = string.Empty;

            foreach(string path in files)
                if(path.EndsWith(".zip"))
                {
                    rightPath = path;
                    break;
                }    

            byte[] fileBytes = System.IO.File.ReadAllBytes(rightPath);

            return File(fileBytes, "application/force-download", "forester.zip");
        }
    }
}
