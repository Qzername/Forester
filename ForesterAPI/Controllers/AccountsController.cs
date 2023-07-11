using ForesterAPI.Data;
using ForesterAPI.Models;
using ForesterAPI.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

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
            key = configuration.GetValue<string>("Jwt:Key")!;

            this.accountDatabase = accountDatabase;
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public IActionResult Login(Account account)
        {
            if (string.IsNullOrEmpty(account.Login) || string.IsNullOrEmpty(account.Password))
                return StatusCode(406);

            if (account.Login.Length > 20 || account.Password.Length > 20)
                return StatusCode(400);

            if (!accountDatabase.DoesExist(account.Login))
                return StatusCode(404);

            Account user = accountDatabase.Get(account.Login);

            if (user.Password != EncryptionManager.Encrypt(account.Password))
                return StatusCode(403);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials,
                claims: new Claim[]
                {
                    new Claim("ID", user.ID.ToString()),
                    new Claim("Login", user.Login),
                }
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new JWT()
            {
                Token = tokenString,
            });
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public IActionResult Register(Account account)
        {
            if (string.IsNullOrEmpty(account.Login) || string.IsNullOrEmpty(account.Username) || string.IsNullOrEmpty(account.Password))
                return StatusCode(406);

            if (account.Login.Length > 20 || account.Username.Length > 20 || account.Password.Length > 20)
                return StatusCode(400);

            if (accountDatabase.DoesExist(account.Login))
                return StatusCode(499); //Already in base

            accountDatabase.Create(account);

            return Ok();
        }

        [HttpPut("[action]")]
        public IActionResult Update(Account account) 
        {
            if (!accountDatabase.DoesExist(account.Login))
                return StatusCode(404);

            accountDatabase.Update(account);
            return Ok();
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public IActionResult GetById([FromQuery] int id) 
        {
            if (!accountDatabase.DoesExist(id))
                return StatusCode(404);

            var account = accountDatabase.Get(id);

            account.Password = string.Empty;

            return Ok(account); 
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public IActionResult GetByLogin([FromQuery] string login) 
        {
            if (!accountDatabase.DoesExist(login))
                return StatusCode(404);

            var account = accountDatabase.Get(login);

            account.Password = string.Empty;

            return Ok(account);
        }
    }
}
