using ForesterAPI.Data;
using ForesterAPI.Data.Connection;
using ForesterAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace ForesterAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ApplicationFilesController : ControllerBase
    {
        ApplicationDatabase applicationDatabase;
        FileDatabase fileDatabase;
        RequirementChecker requirementChecker;

        public ApplicationFilesController(ApplicationDatabase applicationDatabase, FileDatabase fileDatabase, RequirementChecker requirementChecker)
        {
            this.applicationDatabase = applicationDatabase;
            this.fileDatabase = fileDatabase;
            this.requirementChecker = requirementChecker;
        }

        [HttpPost("[action]")]
        public IActionResult Download([FromQuery] string name, [FromBody] Dictionary<string, string> doNotInclude)
        {
            if (!applicationDatabase.DoesExist(name))
                return StatusCode(403);

            string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (!requirementChecker.IsAllowedRequirement(login, name))
                return StatusCode(403);

            var application = applicationDatabase.Get(name);

            if (!fileDatabase.DoesApplicationExist(name))
                return StatusCode(403);

            applicationDatabase.IncrementDownloadNumber(application);

            return File(fileDatabase.ExtractApplicationFiles(name, doNotInclude), "application/force-download", name + ".zip");
        }

        [HttpPost("[action]")]
        [DisableRequestSizeLimit]
        public IActionResult Upload([FromQuery] string name, IFormFile file)
        {
            if (!applicationDatabase.DoesExist(name))
                return StatusCode(403);

            string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (!requirementChecker.IsAllowedRequirement(login, name))
                return StatusCode(403);

            fileDatabase.CreateApplication(name, file);

            return Ok();
        }

        [HttpGet("[action]")]
        public IActionResult GetChecksum(string name)
        {
            if (!applicationDatabase.DoesExist(name))
                return StatusCode(403);

            string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (!requirementChecker.IsAllowedRequirement(login, name))
                return StatusCode(403);

            var application = applicationDatabase.Get(name);

            if (!fileDatabase.DoesApplicationExist(name))
                return StatusCode(403);

            return Ok(fileDatabase.GetChecksum(name));
        }
    }
}
