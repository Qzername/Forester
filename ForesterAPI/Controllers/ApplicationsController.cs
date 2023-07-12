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

            application.Owner = account.ID;

            applicationDatabase.Create(application);

            return Ok();
        }

        [HttpPut]
        public IActionResult Put([FromQuery] string name, Application application) 
        {
            string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (!requirementChecker.DoesOwnRequirement(login, name))
                return StatusCode(403);

            applicationDatabase.Update(name, application);

            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete(string applicationName)
        {
            string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (!requirementChecker.DoesOwnRequirement(login, applicationName))
                return StatusCode(403);

            applicationDatabase.Delete(applicationName);

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

        [HttpPost("[action]")]
        public IActionResult GetApplicationAllowed(Application application)
        {
            string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (!requirementChecker.DoesOwnRequirement(login, application.Name))
                return StatusCode(403);

            return Ok(applicationDatabase.GetApplicationAllowed(application));
        }

        [HttpPost("[action]")]
        public IActionResult GetApplicationDevelopers(Application application)
        {
            string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (!requirementChecker.DoesOwnRequirement(login, application.Name))
                return StatusCode(403);

            return Ok(applicationDatabase.GetApplicationDevelopers(application));
        }

        [HttpPost("[action]")]
        public IActionResult ChangePermission(PermissionChangeData data)
        {
            string ownerLogin = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (!requirementChecker.DoesOwnRequirement(ownerLogin, data.ApplicationName))
                return StatusCode(403);

            var account = accountDatabase.Get(data.AccountLogin);

            var application = applicationDatabase.Get(data.ApplicationName);

            applicationDatabase.ChangePermission(application, account, data.Permission);

            return Ok();
        }
    }
}
