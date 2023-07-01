using ForesterAPI.Data;
using ForesterAPI.Models.Picture;
using ForesterAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForesterAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PicturesController : ControllerBase
    {
        ApplicationDatabase applicationDatabase;
        FileDatabase fileDatabase;
        RequirementChecker requirementChecker;

        public PicturesController(ApplicationDatabase applicationDatabase,FileDatabase fileDatabase, RequirementChecker requirementChecker)
        {
            this.applicationDatabase = applicationDatabase;
            this.fileDatabase = fileDatabase;
            this.requirementChecker = requirementChecker;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string name, [FromQuery] ObjectType objectType, [FromQuery] PictureType pictureType)
        {
            if(!fileDatabase.DoesPictureExist(objectType,name, pictureType))
                return NotFound();

            if(objectType == ObjectType.Account)
            {
                var application = applicationDatabase.Get(name);
                string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
         
                if(application.IsPrivate && !requirementChecker.IsAllowedRequirement(login,name))
                    return NotFound();
            }

            return File(fileDatabase.GetPicture(objectType, name, pictureType), "image/png");
        }

        [HttpPut]
        public IActionResult Put([FromQuery] string name, [FromQuery] ObjectType objectType, [FromQuery] PictureType pictureType, IFormFile file)
        {
            string login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (objectType == ObjectType.Account && name != login)
                return StatusCode(403);

            if (objectType == ObjectType.Application && applicationDatabase.DoesExist(name) && requirementChecker.DoesOwnRequirement(login, name))
                return StatusCode(404);

            fileDatabase.UpdatePicture(objectType, name, pictureType, file);

            return Ok();
        }
    }
}
