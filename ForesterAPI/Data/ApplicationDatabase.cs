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
        FileDatabase fileDatabase;

        public ApplicationDatabase(SQLManager sqlManager, FileDatabase fileDatabase)
        {
            this.sqlManager = sqlManager;
            this.fileDatabase = fileDatabase;
        }

        public Application[] Get(Account account) 
        { 
            List<Application> applications = new List<Application>();

            //all public
            applications.AddRange(sqlManager.SelectMany<Application>("SELECT * FROM Applications WHERE IsPrivate=0"));

            //all allowed
            applications.AddRange(GetAllowed(account).Where(x => x.IsPrivate == true));

            return applications.ToArray();
        }

        public Application Get(int id) => sqlManager.SelectSingle<Application>($"SELECT * FROM Applications WHERE ID={id}");
        public Application Get(string name) => sqlManager.SelectSingle<Application>(@$"SELECT * FROM Applications WHERE Name=""{name}""");
        
        /// <summary>
        /// By default, applications will be set to private
        /// </summary>
        public void Create(Application application) => sqlManager.ExecuteNonQuery(@$"INSERT INTO Applications(Owner, Name, Version, IsPrivate) VALUES(""{application.Owner}"", ""{application.Name}"", ""None"", 1)");

        public void Update(string oldName, Application application)
        {
            string query = "UPDATE Applications SET ";

            if (!string.IsNullOrEmpty(application.Name))
                if (sqlManager.SelectSingleValue<int>(@$"SELECT COUNT(Name) FROM Applications WHERE Name=""{application.Name}""") > 0)
                    throw new Exception("User requested application's name change but name is taken");
                else
                {
                    query += @$"Name = ""{application.Name}"",";
                    fileDatabase.Rename(oldName, application.Name);
                }

            if (!string.IsNullOrEmpty(application.Description))
                query += @$"Description = ""{application.Description}"",";

            if (!string.IsNullOrEmpty(application.ShortDescription))
                query += @$"ShortDescription = ""{application.ShortDescription}"",";

            if (!string.IsNullOrEmpty(application.Version))
                query += @$"Version = ""{application.Version}"",";

            if (sqlManager.SelectSingleValue<bool>(@$"SELECT IsPrivate FROM Applications WHERE Name = ""{oldName}""") != application.IsPrivate)
                query += $@"IsPrivate = ""{Convert.ToInt32(application.IsPrivate)}"",";

            if (query[^1] != ',')
                return;

            query = query.Remove(query.Length - 1);
            query += $@" WHERE Name = ""{oldName}""";

            sqlManager.ExecuteNonQuery(query);
        }

        public bool DoesExist(string applicationName) => Convert.ToBoolean(sqlManager.SelectSingleValue<int>(@$"SELECT COUNT(ID) FROM Applications WHERE Name = ""{applicationName}"""));
        public bool DoesExist(int applicationID) => Convert.ToBoolean(sqlManager.SelectSingleValue<int>(@$"SELECT COUNT(ID) FROM Applications WHERE ID = {applicationID}"));

        public void Delete(string name) 
        {
            sqlManager.ExecuteNonQuery(@$"DELETE FROM Applications WHERE Name=""{name}""");
            fileDatabase.Delete(name);
        }

        public void IncrementDownloadNumber(Application application) => sqlManager.ExecuteNonQuery(@$"UPDATE Applications SET DownloadNumber = DownloadNumber + 1 WHERE Name=""{application.Name}""");

        public Application[] GetOwned(Account owner) => sqlManager.SelectMany<Application>($"SELECT * FROM Applications WHERE Owner={owner.ID}");

        /// <summary>
        /// Gets every application that user owns
        /// </summary>
        public Application[] GetDeveloped(Account owner)
        {
            List<Application> applications = new List<Application>();

            //where user is owner
            applications.AddRange(GetOwned(owner));

            //where user is developer
            applications.AddRange(sqlManager.SelectMany<Application>($"SELECT Applications.* FROM Applications, Permissions WHERE Permissions.AccountID = {owner.ID} AND Applications.ID = Permissions.ApplicationID AND Permission=1"));
            //about section "Permission=1" please read note at the start of class

            return applications.ToArray();
        }

        /// <summary>
        /// Gets every application that user is allowed to see
        /// Counts also applications where user is developer
        /// </summary>
        public Application[] GetAllowed(Account account)
        {
            List<Application> applications = new List<Application>();

            applications.AddRange(GetDeveloped(account));

            //where user is allowed
            applications.AddRange(sqlManager.SelectMany<Application>($"SELECT Applications.* FROM Applications, Permissions WHERE Permissions.AccountID = {account.ID} AND Applications.ID = Permissions.ApplicationID AND Permission=0"));
            //about section "Permission=1" please read note at the start of class

            return applications.ToArray();
        }

        public Account[] GetApplicationAllowed(Application application) => GetUsersByPermission(application, Permission.Allowed);

        public Account[] GetApplicationDevelopers(Application application) => GetUsersByPermission(application, Permission.Developer);

        Account[] GetUsersByPermission(Application application, Permission permission) => sqlManager.SelectMany<Account>($"SELECT Accounts.* FROM Accounts, Permissions WHERE Permissions.Permission = {(int)permission} AND Accounts.ID = Permissions.AccountID AND Permissions.ApplicationID = {application.ID}");

        /// <summary>
        /// Can also remove permissions with Permission.Remove
        /// </summary>
        public void ChangePermission(Application application, Account account, Permission permission)
        {
            if(permission == Permission.Remove)
            {
                sqlManager.ExecuteNonQuery($"DELETE FROM Permissions WHERE ApplicationID = {application.ID} AND AccountID = {account.ID}");
                return;
            }

            if (PermissionExists(application, account))
                sqlManager.ExecuteNonQuery($"UPDATE Permissions SET Permission = {(int)permission} WHERE ApplicationID = {application.ID} AND AccountID = {account.ID}"); //update current permission
            else
                sqlManager.ExecuteNonQuery($"INSERT INTO Permissions(ApplicationID, AccountID, Permission) VALUES({application.ID},{account.ID},{(int)permission})"); //create new permission
        }

        bool PermissionExists(Application application, Account account) => Convert.ToBoolean(sqlManager.SelectSingleValue<int>(@$"SELECT COUNT(ApplicationID) FROM Permissions WHERE ApplicationID = {application.ID} AND AccountID = {account.ID}"));
    }
}
