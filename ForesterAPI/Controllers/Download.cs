using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ForesterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Download : ControllerBase
    {
        // GET: api/<Download>
        [HttpGet]
        public FileContentResult DownloadDocument()
        {
            string filePath = "./test.zip";
            string fileName = "test.zip";

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/force-download", fileName);
        }
    }
}
