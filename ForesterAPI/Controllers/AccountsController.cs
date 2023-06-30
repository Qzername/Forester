using ForesterAPI.Data;
using ForesterAPI.Models;
using ForesterAPI.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ForesterAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {
        AccountDatabase accountDatabase;
        string key;

        public AccountsController(IConfiguration configuration, AccountDatabase accountDatabase)
        {
            key = configuration.GetValue<string>("Jwt:Key");

            this.accountDatabase = accountDatabase;
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public IActionResult Login(Account account)
        {
            return Ok();
        }

        [HttpPost("[action]")]
        public IActionResult Register(Account account)
        {
            accountDatabase.Create(new Account()
            {
                Login = "***REMOVED***name",
                Username = "uZer",
                Password = "123dupa123",
            });

            return Ok();
        }

        [HttpPut("[action]")]
        public IActionResult Update(Account account) 
        {
            return Ok();
        }

        [HttpGet]
        public IActionResult Get([FromQuery]int? id, [FromQuery] string? login)
        {
            if (id is null)
                return Ok();
            
            if(login is null)
                return Ok();

            var account = accountDatabase.Get(id.Value);

            Debug.WriteLine("-===--===-");
            Debug.WriteLine(account.ID);
            Debug.WriteLine(account.Login);
            Debug.WriteLine(account.Password);
            Debug.WriteLine(account.IsDeveloper);
            Debug.WriteLine("-===--===-");

            return Ok(account);
        }
    }
}
