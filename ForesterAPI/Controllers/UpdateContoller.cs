using ForesterAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;

namespace ForesterAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UpdateContoller : ControllerBase
    {
        string uploadToken;
        ForesterFileDatabase foresterFileDatabase;

        public UpdateContoller(IConfiguration configuration, ForesterFileDatabase foresterFileDatabase)
        {
            uploadToken = configuration.GetValue<string>("UploadForesterToken");
            this.foresterFileDatabase = foresterFileDatabase;
        }

        [HttpGet("[action]")]
        public IActionResult Version() => Ok(foresterFileDatabase.GetVersion());

        [HttpGet("[action]")]
        public IActionResult Checksum() => Ok(foresterFileDatabase.GetChecksum());

        [HttpPost("[action]")]
        public FileContentResult Download([FromBody] Dictionary<string, string> doNotInclude) => File(foresterFileDatabase.GetForesterFiles(doNotInclude), "application/force-download", "forester.zip");

        [HttpPost("Forester/UploadNewVersion")]
        [DisableRequestSizeLimit]
        public IActionResult Upload([FromHeader] string token, [FromQuery] string version, IFormFile file)
        {
            if (token != uploadToken)
                return StatusCode(403);

            foresterFileDatabase.SetForesterFiles(version, file);

            return Ok();
        }
    }
}
