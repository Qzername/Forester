using Microsoft.AspNetCore.Mvc;
using ForesterAPI.Models;
using System.Data.SQLite;

namespace ForesterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        // GET: api/<AccountsController>/Login
        [HttpPost("[action]")]
        public IActionResult Login(LoginCredentials login)
        {
            if (string.IsNullOrEmpty(login.username) || string.IsNullOrEmpty(login.password))
                return StatusCode(406);

            if (login.password.Length > 20 || login.username.Length > 20)
                return StatusCode(400);

            var users = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE username=\"{login.username}\" AND password=\"{login.password}\"");

            if (users.Length == 0)
                return StatusCode(404);

            long expTime = DateTimeOffset.UtcNow.AddDays(7).ToUnixTimeSeconds();

            Dictionary<string, object> claims = new Dictionary<string, object>() { { "ID", users[0].ID }, { "username", users[0].friendlyUsername } };
            Token token = new Token() { exp = expTime, token = JWTManager.Encode(claims) };

            return Ok(JSONManager.Serialize(token));
        }

        //POST: api/<AccountsController>/Register
        [HttpPost("[action]")]
        public IActionResult Register(RegisterCredentials register)
        {
            if (string.IsNullOrEmpty(register.username) || string.IsNullOrEmpty(register.password) || string.IsNullOrEmpty(register.loginUsername)) 
                return StatusCode(406);

            if (register.password.Length > 20 || register.username.Length > 20 || register.loginUsername.Length > 20)
                return StatusCode(400);

            if (SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE username=\"{register.loginUsername}\"").Length > 0)
                return StatusCode(499); //Already in base

            SQLDatabase.NoReturnQuery($"INSERT INTO Accounts(friendly_ID, friendly_username, username, password, isDeveloper) VALUES(\"{RandomFriendlyID(register.username)}\",\"{register.username}\",\"{register.loginUsername}\",\"{register.password}\",\"{false}\")");

            return Ok();
        }

        [HttpPut("[action]")]
        public IActionResult Update([FromHeader] string token, Account update)
        {
            string decoded = JWTManager.Decode(token);

            if (decoded == "")
                return StatusCode(403);

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);

            string query = "UPDATE Accounts SET ";

            if(!string.IsNullOrEmpty(update.friendlyUsername))
                query += "friendly_username = \"" + update.friendlyUsername + "\",";

            if(!string.IsNullOrEmpty(update.password))
                query += "password = \"" + update.password + "\","; 
            
            if(!string.IsNullOrEmpty(update.description))
                query += "description = \"" + update.description + "\",";

            if(query[^1] != ',')
                return StatusCode(406);

            query = query.Remove(query.Length - 1);
            query += " WHERE username = \"" + loginToken.username + "\"";

            SQLDatabase.NoReturnQuery(query);

            return Ok();

        }

        [HttpGet("[action]")]
        public IActionResult GetUser([FromQuery] string? username, [FromQuery] string? friendlyUsername, [FromQuery] long? id)
        { 
            var users = SQLDatabase.Select<Account>("SELECT * FROM Accounts WHERE " + 
                (friendlyUsername is null?"": $"friendly_username=\"{friendlyUsername}\" ") +
                (username is null?"": $"username=\"{username}\" ") +
                (id is null ?"": $"ID={id} "));;

            if (users.Length == 0)
                return StatusCode(404);

            var user = users[0];

            user.username = "";
            user.password = "";

            return Ok(JSONManager.Serialize(user));
        }

        int RandomFriendlyID(string username)
        {
            var accs = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE friendly_username=\"{username}\"");

            List<int> excludedNumbers = new List<int>();

            foreach (Account acc in accs)
                excludedNumbers.Add(acc.friendly_ID);

            var range = Enumerable.Range(1001, 9999).Where(i => !excludedNumbers.Contains(i));

            int index = new Random().Next(1000, 9999 - excludedNumbers.Count);

            return range.ElementAt(index);
        }
    }
}
