using ForesterAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForesterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        // GET api/<ApplicationsController>/Get
        [HttpGet("[action]")]
        public IActionResult Get([FromHeader] string token)
        {
            //Deserializacja tokenu
            string decoded = JWTManager.Decode(token);

            if (decoded == "")
                return StatusCode(403);

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);
            
            //Tworzenie tabeli zwrotnej
            List<Application> applications = new List<Application>();

            //Sprawdzenie czy użytkownik jest deweloperem
            if(SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE ID={loginToken.ID} AND isDeveloper=\"True\"").Length > 0)
                //Dodanie aplikacji gdzie użytkownik jest super deweloperem (od razu sprawdzam czy są prywatne)
                applications.AddRange(SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE mainDeveloper = {loginToken.ID} AND isPrivate = \"True\""));

            //Dodanie aplikacji gdzie użytkownik jest deweloperem (od razu sprawdzam czy są prywatne)
            applications.AddRange(SQLDatabase.Select<Application>($"SELECT Applications.* FROM Applications, Developers WHERE Developers.ID_User = {loginToken.ID} AND Applications.ID = Developers.ID_Application AND Applications.isPrivate = \"True\""));

            //Dodanie prywatnych aplikacji gdzie użytkownik ma dostęp
            applications.AddRange(SQLDatabase.Select<Application>($"SELECT Applications.* FROM Applications, AllowedUsers WHERE AllowedUsers.ID_User = {loginToken.ID} AND Applications.ID = AllowedUsers.ID_Application AND Applications.isPrivate = \"True\""));

            //Dodanie reszty aplikacji
            applications.AddRange(SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE isPrivate=\"False\""));

            //Usuwanie duplikatów
            applications = applications.Distinct().ToList();

            return Ok(JSONManager.Serialize(applications.ToArray()));
        }

        // GET api/<ApplicationsController>/GetSingle
        [HttpGet("[action]")]
        public IActionResult GetSingle([FromHeader] string token, [FromQuery] string? name, [FromQuery] string? id)
        {
            //Deserializacja tokenu
            string decoded = JWTManager.Decode(token);

            if (decoded == "")
                return StatusCode(403);

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);

            bool nameIsNull = string.IsNullOrEmpty(name);
            bool idIsNull = string.IsNullOrEmpty(id);

            if (nameIsNull && idIsNull)
                return StatusCode(403);

            string query = "SELECT * FROM Applications WHERE " + (nameIsNull ? "" : $"name =\"{name}\"") + (!idIsNull && !nameIsNull ? " AND " : "") + (idIsNull ? "" : $"id={id}");

            //Wyciągnięcie aplikacji
            var apps = SQLDatabase.Select<Application>(query);

            if (apps.Length == 0)
                return StatusCode(404);

            var app = apps[0];

            //Aplikacja jest prywatna, wymagana jest weryfikacja czy użytkownik ma do niej dostęp
            if (app.isPrivate == "True" && app.mainDeveloper != loginToken.ID && SQLDatabase.Select<Application>($"SELECT Applications.* FROM Applications, Developers WHERE Applications.ID = {app.ID} AND Developers.ID_User = {loginToken.ID}").Length == 0)
                return StatusCode(404); //Serwer zwraca wartość że nie wie o jaką aplikacje chodzi w celu ochrony danych o istnieniu aplikacji w bazach

            return Ok(JSONManager.Serialize(app));
        }

        // GET api/<ApplicationsController>/GetDeveloped
        [HttpGet("[action]")]
        public IActionResult GetDeveloped([FromHeader] string token)
        {
            //Deserializacja tokenu
            string decoded = JWTManager.Decode(token);

            if (decoded == "")
                return StatusCode(403);

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);

            //Tworzenie tabeli zwrotnej
            List<Application> applications = new List<Application>();

            //Dodanie aplikacji gdzie użytkownik jest super deweloperem
            applications.AddRange(SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE mainDeveloper = {loginToken.ID}"));

            //Dodanie aplikacji gdzie użytkownik jest deweloperem
            applications.AddRange(SQLDatabase.Select<Application>($"SELECT Applications.* FROM Applications, Developers WHERE Developers.ID_User = {loginToken.ID}  AND Applications.ID = Developers.ID_Application"));

            return Ok(JSONManager.Serialize(applications.ToArray()));
        }

        // GET api/<ApplicationsController>/GetAllowed
        [HttpGet("[action]")]
        public IActionResult GetAllowed([FromHeader] string token, [FromQuery] string name)
        {
            //Deserializacja tokenu
            string decoded = JWTManager.Decode(token);

            if (decoded == "")
                return StatusCode(403);

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);

            var apps = SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE name=\"{name}\" AND mainDeveloper={loginToken.ID}");

            if(apps.Length == 0)
                return StatusCode(403);

            var users = SQLDatabase.Select<Account>($"SELECT Accounts.* FROM Accounts, AllowedUsers WHERE AllowedUsers.ID_Application = {apps[0].ID} AND AllowedUsers.ID_User = Accounts.ID");

            for(int i = 0; i < users.Length; i++)
            {
                users[i].username = "";
                users[i].password = "";
            }

            var developers = SQLDatabase.Select<Account>($"SELECT Accounts.* FROM Accounts, Developers WHERE Developers.ID_Application = {apps[0].ID} AND Developers.ID_User = Accounts.ID");

            for (int i = 0; i < developers.Length; i++)
            {
                developers[i].username = "";
                developers[i].password = "";
            }

            Allowed allowed = new Allowed()
            {
                name = name,
                allowedUsers = users,
                allowedDevelopers = developers
            };

            return Ok(JSONManager.Serialize(allowed));
        }

        // PUT api/<ApplicationsController>/UpdateAllowed
        [HttpPut("[action]")]
        public IActionResult UpdateAllowed([FromHeader] string token, [FromBody] Allowed allowed)
        {
            //Deserializacja tokenu
            string decoded = JWTManager.Decode(token);

            if (decoded == "")
                return StatusCode(403);

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);

            var apps = SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE name=\"{allowed.name}\" AND mainDeveloper={loginToken.ID}");

            if (apps.Length == 0)
                return StatusCode(403);

            if (allowed.allowedDevelopers is null)
                allowed.allowedDevelopers = new Account[0];

            if (allowed.allowedUsers is null)
                allowed.allowedUsers = new Account[0];

            List<long> allowedUsersID = new List<long>();

            foreach(Account account in allowed.allowedUsers)
            {
                var id = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE friendly_username = \"{account.friendlyUsername}\" AND friendly_ID = {account.friendly_ID}");

                if (id.Length == 0)
                    return StatusCode(404);

                allowedUsersID.Add(long.Parse(id[0].ID.ToString()));
            }

            List<long> allowedDevelopersID = new List<long>();

            foreach (Account account in allowed.allowedDevelopers)
            {
                var id = SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE friendly_username = \"{account.friendlyUsername}\" AND friendly_ID = {account.friendly_ID}");

                if (id.Length == 0)
                    return StatusCode(404);

                allowedDevelopersID.Add(long.Parse(id[0].ID.ToString()));
            }

            long appID = apps[0].ID;

            SQLDatabase.NoReturnQuery($"DELETE FROM AllowedUsers WHERE ID_Application = {appID}");

            foreach (long id in allowedUsersID)
                SQLDatabase.NoReturnQuery($"INSERT INTO AllowedUsers(ID_Application, ID_User) VALUES({appID},{id})");

            SQLDatabase.NoReturnQuery($"DELETE FROM Developers WHERE ID_Application = {appID}");

            foreach (long id in allowedDevelopersID)
                SQLDatabase.NoReturnQuery($"INSERT INTO Developers(ID_Application, ID_User) VALUES({appID},{id})");

            return Ok();
        }

        // POST api/<ApplicationsController>/New
        [HttpPost("[action]")]
        public IActionResult New([FromHeader] string token, Application app)
        {
            //Deserializacja tokenu
            string decoded = JWTManager.Decode(token);

            if (decoded == "")
                return StatusCode(403);

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);

            if (SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE ID={loginToken.ID} AND isDeveloper=\"True\"").Length == 0)
                return StatusCode(403);

            if (SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE name=\"{app.name}\"").Length > 0)
                return StatusCode(499);

            SQLDatabase.NoReturnQuery($"INSERT INTO Applications(name, version, isPrivate, mainDeveloper) VALUES(\"{app.name}\", \"{app.version}\", \"False\", {loginToken.ID})");

            return Ok();
        }

        // PUT api/<ApplicationsController>/Update
        [HttpPut("[action]")]
        public IActionResult Update([FromHeader] string token, [FromQuery] string name, Application app)
        {
            //Deserializacja tokenu
            string decoded = JWTManager.Decode(token);

            if (decoded == "")
                return StatusCode(403);

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);

            if (SQLDatabase.Select<Account>($"SELECT * FROM Accounts WHERE ID={loginToken.ID} AND isDeveloper=\"True\"").Length == 0)
            {
                var selectApp = SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE name=\"{name}\"");

                if (selectApp.Length == 0)
                    return StatusCode(404);

                if(SQLDatabase.Select<ulong>($"SELECT ID_User FROM Developers WHERE ID_Application = {selectApp[0].ID} AND ID_User = {loginToken.ID}").Length != 0 && !string.IsNullOrEmpty(app.version))
                {
                    SQLDatabase.NoReturnQuery($"UPDATE Applications SET version = \"{app.version}\" WHERE name = \"{name}\"");
                    return Ok();
                }

                return StatusCode(403);
            }

            var selectedApps = SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE name=\"{name}\" AND mainDeveloper={loginToken.ID}");

            if (selectedApps.Length == 0)
                return StatusCode(403);

            //Podmiana danych oraz zapis
            string query = "UPDATE Applications SET ";

            if (!string.IsNullOrEmpty(app.name))
                if (SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE name=\"{app.name}\"").Length > 0)
                    return StatusCode(499);
                else
                {
                    ApplicationManager.Rename(name, app.name);
                    query += "name = \"" + app.name + "\",";
                }

            if (!string.IsNullOrEmpty(app.description)) 
                query += "description = \"" + app.description + "\",";

            if (!string.IsNullOrEmpty(app.quickDescription))
                query += "quickDescription = \"" + app.quickDescription + "\",";

            if (!string.IsNullOrEmpty(app.version))
                query += "version = \"" + app.version + "\",";

            if (!string.IsNullOrEmpty(app.isPrivate))
                query += "isPrivate = \"" + app.isPrivate + "\",";

            if (query[^1] != ',')
                return StatusCode(406);

            query = query.Remove(query.Length - 1);
            query += $" WHERE name = \"{name}\"";

            SQLDatabase.NoReturnQuery(query);

            return Ok();
        }
    }
}
