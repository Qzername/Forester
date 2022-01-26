using Microsoft.AspNetCore.Mvc;
using ForesterAPI.Models;
using System.Data.SQLite;
using ForesterAPI.Database;

namespace ForesterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        // GET: api/<AccountsController>/Login
        [HttpGet("[action]")]
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
            if (string.IsNullOrEmpty(register.username) || string.IsNullOrEmpty(register.password))
                return StatusCode(406);

            if (register.password.Length > 20 || register.username.Length > 20)
                return StatusCode(400);

            if (SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE username=\"{register.loginUsername}\"").Length > 0)
                return StatusCode(499); //Already in base

            SQLDatabase.NoReturnQuery($"INSERT INTO Accounts(friendly_ID, friendly_username, username, password, isDeveloper) VALUES(\"{RandomFriendlyID(register.username)}\",\"{register.password}\",\"{register.username}\",\"{register.password}\",\"{false}\")");

            return Ok();
        }

        [HttpPut("[action]")]
        public IActionResult Update(UpdateCredentials update)
        {
            string decoded = JWTManager.Decode(update.token);

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
        public FileContentResult GetPicture(UpdatePictureCredentials update)
        {
            var users = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE username=\"{update.name}\"");

            if (users.Length == 0)
                return null;

            var type = (PictureManager.Picture)Enum.Parse(typeof(PictureManager.Picture), update.pictureType);

            (bool profilePicture, bool backgroundPicture) = PictureManager.CheckIfExist(update.name);

            if ((type == PictureManager.Picture.profile && profilePicture) || (type == PictureManager.Picture.background && backgroundPicture))
                return null; 

            return File(PictureManager.GetImage(update.name, type), "image/png");
        }

        [HttpPut("[action]")]
        public IActionResult UpdatePicture(UpdatePictureCredentials update, IFormFile file)
        {
            string decoded = JWTManager.Decode(update.name);

            if (decoded == "")
                return StatusCode(403);

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();

                PictureManager.UpdateImage(loginToken.username, (PictureManager.Picture)Enum.Parse(typeof(PictureManager.Picture), update.pictureType), fileBytes);
            }

            return Ok();
        }

        [HttpGet("[action]")]
        public IActionResult GetUser(string username)
        {
            var users = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE username=\"{username}\"");

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
