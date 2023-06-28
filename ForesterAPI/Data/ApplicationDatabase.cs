using ForesterAPI.Data.Connection;
using ForesterAPI.Models;
using System.Xml.Linq;

namespace ForesterAPI.Data
{
    /// <summary>
    /// Every tool thats needed to communicate with application database
    /// Does not verify data!
    /// </summary>
    public class ApplicationDatabase
    {
        /*
         * Note:
         * Permissions table contains permission value.
         * It is numerical because i would want to implement in future custom role system
         * Currently 0 is allowed and 1 is developer
         */

        SQLManager sqlManager;

        public ApplicationDatabase(SQLManager SQLManager)
        {
            sqlManager = SQLManager;
        }

        public Application[] Get() => sqlManager.SelectMany<Application>("SELECT * FROM Applications");
        public Application Get(int ID) => sqlManager.SelectSingle<Application>($"SELECT * FROM Applications WHERE ID={ID}");
        public Application Get(string Name) => sqlManager.SelectSingle<Application>(@$"SELECT * FROM Applications WHERE Name=""{Name}""");
        
        /// <summary>
        /// By default, applications will be set to private
        /// </summary>
        public void Create(Application Application) => sqlManager.ExecuteNonQuery(@$"INSERT INTO Applications(Owner, Name, Version, IsPrivate) VALUES(""{Application.Owner}"", ""{Application.Name}"", ""None"", 0)");

        public void Update(Application Application)
        {
            string query = "UPDATE Applications SET ";

            if (!string.IsNullOrEmpty(Application.Name))
                if (sqlManager.SelectSingle<int>(@$"SELECT COUNT(Name) FROM Applications WHERE Name=""{Application.Name}""") > 0)
                    throw new Exception("User requested application's name change but name is taken");
                else
                    query += @$"Name = ""{Application.Name}"",";

            if (!string.IsNullOrEmpty(Application.Description))
                query += @$"Description = ""{Application.Description}"",";

            if (!string.IsNullOrEmpty(Application.ShortDescription))
                query += @$"ShortDescription = ""{Application.ShortDescription}"",";

            if (!string.IsNullOrEmpty(Application.Version))
                query += @$"Version = ""{Application.Version}"",";

            if(sqlManager.SelectSingle<bool>($"SELECT IsPrivate FROM Applications WHERE Name = {Application.Name}"))
                query += $@"IsPrivate = ""{Convert.ToInt32(Application.IsPrivate)}"",";

            if (query[^1] != ',')
                return;

            query = query.Remove(query.Length - 1);
            query += $@" WHERE Name = ""{Application.Name}""";

            sqlManager.ExecuteNonQuery(query);
        }

        public void IncrementDownloadNumber(Application Application) => sqlManager.ExecuteNonQuery(@$"UPDATE Applications SET DownloadNumber = DownloadNumber + 1 WHERE Name=""{Application.Name}""");

        /// <summary>
        /// Gets every application that user owns
        /// </summary>
        public Application[] GetDeveloped(Account Owner)
        {
            List<Application> applications = new List<Application>();

            //where user is owner
            applications.AddRange(sqlManager.SelectMany<Application>($"SELECT * FROM Applications WHERE Owner={Owner.ID}"));

            //where user is developer
            applications.AddRange(sqlManager.SelectMany<Application>($"SELECT Applications.* FROM Applications, Permissions WHERE Permissions.AccountID = {Owner.ID} AND Applications.ID = Permissions.ApplicationID AND Permission=1"));
            //about section "Permission=1" please read note at the start of class

            return applications.ToArray();
        }

        public bool DoesDevelop(Application Application, Account Account) => GetDeveloped(Account).Any(x=>x.Name ==  Application.Name);

        /// <summary>
        /// Gets every application that user is allowed to see
        /// Counts also applications where user is developer
        /// </summary>
        public Application[] GetAllowed(Account Account)
        {
            List<Application> applications = new List<Application>();

            applications.AddRange(GetDeveloped(Account));

            //where user is allowed
            applications.AddRange(sqlManager.SelectMany<Application>($"SELECT Applications.* FROM Applications, Permissions WHERE Permissions.AccountID = {Account.ID} AND Applications.ID = Permissions.ApplicationID AND Permission=0"));
            //about section "Permission=1" please read note at the start of class

            return applications.ToArray();
        }

        /// <summary>
        /// Can also remove permissions with Permission.Remove
        /// </summary>
        public void ChangePermission(Application Application, Account Account, Permission Permission)
        {
            if(Permission == Permission.Remove)
            {
                sqlManager.ExecuteNonQuery($"DELETE FROM Permissions WHERE ApplicationID = {Application.ID} AND AccountID = {Account.ID}");
                return;
            }

            if (PermissionExists(Application, Account))
                sqlManager.ExecuteNonQuery($"UPDATE Permissions SET Permission = {(int)Permission} WHERE ApplicationID = {Application.ID} AND AccountID = {Account.ID}"); //update current permission
            else
                sqlManager.ExecuteNonQuery($"INSERT INTO Permissions(ApplicationID, AccountID, Permission) VALUES({Application.ID},{Account.ID},{(int)Permission})"); //create new permission
        }

        bool PermissionExists(Application Application, Account Account) => Convert.ToBoolean(sqlManager.SelectSingle<int>(@$"SELECT COUNT(ApplicationID) FROM Permissions WHERE ApplicationID = {Application.ID} AND AccountID = {Account.ID}"));
    }
}
