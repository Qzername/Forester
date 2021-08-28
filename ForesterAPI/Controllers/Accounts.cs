using ForesterAPI.Databases;
using ForesterAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace ForesterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Accounts : ControllerBase
    {
        // GET: api/<Accounts>
        [HttpGet("[action]/{name}/{password}")]
        public string GetUser(string name, string password)
        {
            var users = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE name=\"{name}\" AND password=\"{password}\"");

            if (users.Length == 0)
                return "NO USER FOUND.";

            return JsonConvert.SerializeObject(users[0]);
        }

        // POST api/<Accounts>
        [HttpPost("[action]")]
        public string NewUser([FromBody] Account user)
        {
            if (user.password.Length > 20 || user.name.Length>20)
                return "FUCK OFF.";

            if (string.IsNullOrEmpty(user.name) || string.IsNullOrEmpty(user.password) || string.IsNullOrEmpty(user.isDeveloper))
                return "JSON WITHOUT NEEDED INFORMATION.";

            if (SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE name=\"{user.name}\"").Length > 0)
                return "NAME TAKEN.";
            
            SQLDatabase.NoReturnQuery($"INSERT INTO Accounts(name, password, isDeveloper) VALUES(\"{user.name}\",\"{user.password}\",\"{user.isDeveloper}\")");

            return "OK.";
        }
    }
}
