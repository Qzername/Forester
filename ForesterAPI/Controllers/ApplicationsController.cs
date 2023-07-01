using ForesterAPI.Data;
using ForesterAPI.Models;
using ForesterAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace ForesterAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ApplicationsController : ControllerBase
    {
        AccountDatabase accountDatabase;
        ApplicationDatabase applicationDatabase;
        RequirementChecker requirementChecker;

        public ApplicationsController(AccountDatabase accountDatabase, ApplicationDatabase applicationDatabase, RequirementChecker requirementChecker)
        {
            this.accountDatabase = accountDatabase;
            this.applicationDatabase = applicationDatabase;
            this.requirementChecker = requirementChecker;
        }

        [HttpGet]
        public IActionResult Get() 
        {
            string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (!accountDatabase.DoesExist(login))
                return NotFound();

            var account = accountDatabase.Get(login);

            return Ok(applicationDatabase.Get(account));
        }

        [HttpGet("[action]")]
        public IActionResult GetById([FromQuery] int id)
        {
            if(!applicationDatabase.DoesExist(id))
                return NotFound();

            var application = applicationDatabase.Get(id);

            string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (application.IsPrivate && !requirementChecker.IsAllowedRequirement(login, application.Name))
                return NotFound();

            return Ok(application);
        }

        [HttpGet("[action]")]
        public IActionResult GetByName([FromQuery] string name) 
        {
            if (!applicationDatabase.DoesExist(name))
                return NotFound();

            var application = applicationDatabase.Get(name);

            string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (application.IsPrivate && !requirementChecker.IsAllowedRequirement(login, application.Name))
                return NotFound();

            return Ok(application);
        }

        [HttpPost]
        public IActionResult Post(Application application)
        {
            string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (!accountDatabase.DoesExist(login))
                return NotFound();

            var account = accountDatabase.Get(login);

            if (!account.IsDeveloper)
                return StatusCode(403);

            if (applicationDatabase.DoesExist(application.Name))
                return StatusCode(499);

            if (string.IsNullOrEmpty(application.Name))
                return StatusCode(406);

            applicationDatabase.Create(application);

            return Ok();
        }

        [HttpPut]
        public IActionResult Put([FromQuery] string name, Application application) 
        {
            string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (!requirementChecker.DoesDevelopRequirement(login, application.Name))
                return StatusCode(403);

            applicationDatabase.Update(application);

            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete(Application application)
        {
            string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (!requirementChecker.DoesDevelopRequirement(login, application.Name))
                return StatusCode(403);

            applicationDatabase.Delete(application.Name);

            return Ok();
        }

        [HttpGet("[action]")]
        public IActionResult GetDeveloped()
        {
            string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (!accountDatabase.DoesExist(login))
                return NotFound();

            var account = accountDatabase.Get(login);

            return Ok(applicationDatabase.GetDeveloped(account));
        }

        [HttpGet("[action]")]
        public IActionResult GetApplicationAllowed(Application application)
        {
            string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (!requirementChecker.DoesOwnRequirement(login, application.Name))
                return StatusCode(403);

            return Ok(applicationDatabase.GetApplicationAllowed(application));
        }

        [HttpGet("[action]")]
        public IActionResult GetApplicationDevelopers(Application application)
        {
            string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (!requirementChecker.DoesOwnRequirement(login, application.Name))
                return StatusCode(403);

            return Ok(applicationDatabase.GetApplicationDevelopers(application));
        }

        [HttpPost("[action]")]
        public IActionResult ChangePermission(Application application, string accountLogin, Permission permission)
        {
            string ownerLogin = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (!requirementChecker.DoesOwnRequirement(ownerLogin, application.Name))
                return StatusCode(403);

            var account = accountDatabase.Get(accountLogin);

            applicationDatabase.ChangePermission(application, account, permission);

            return Ok();
        }
    }
}
