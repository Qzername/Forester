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
            applications.AddRange(SQLDatabase.Select<Application>($"SELECT Applications.* FROM Applications, Developers WHERE Developers.ID_User = {loginToken.ID} AND Applications.isPrivate = \"True\""));

            //Dodanie reszty aplikacji
            applications.AddRange(SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE isPrivate=\"False\""));

            return Ok(JSONManager.Serialize(applications.ToArray()));
        }

        // GET api/<ApplicationsController>/Get/<name>
        [HttpGet("[action]/{name}")]
        public IActionResult Get([FromHeader] string token, [FromQuery] string name)
        {
            //Deserializacja tokenu
            string decoded = JWTManager.Decode(token);

            if (decoded == "")
                return StatusCode(403);

            var loginToken = JSONManager.Deserialize<LoginToken>(decoded);

            //Wyciągnięcie aplikacji
            var apps = SQLDatabase.Select<Application>($"SELECT * FROM Applications WHERE name=\"{name}\"");

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
            applications.AddRange(SQLDatabase.Select<Application>($"SELECT Applications.* FROM Applications, Developers WHERE Developers.ID_User = {loginToken.ID}"));

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

            var users = SQLDatabase.Select<long>($"SELECT ID_User FROM AllowedUsers WHERE ID_Application = {apps[0].ID}");
            var developers = SQLDatabase.Select<long>($"SELECT ID_User FROM Developers WHERE ID_Application = {apps[0].ID}");

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

            long appID = apps[0].ID;

            SQLDatabase.NoReturnQuery($"DELETE FROM AllowedUsers WHERE ID_Application = {appID}");

            foreach (long id in allowed.allowedUsers)
                SQLDatabase.NoReturnQuery($"INSERT INTO AllowedUsers(ID_Application, ID_User) VALUES({appID},{id})");

            SQLDatabase.NoReturnQuery($"DELETE FROM Developers WHERE ID_Application = {appID}");

            foreach (long id in allowed.allowedDevelopers)
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
                return StatusCode(403);

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
