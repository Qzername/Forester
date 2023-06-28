using ForesterAPI.Data.Connection;
using ForesterAPI.Models;
using Microsoft.Win32;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace ForesterAPI.Data
{
    /// <summary>
    /// Every tool thats needed to communicate with account database
    /// Does not verify data!
    /// </summary>
    public class AccountDatabase
    {
        SQLManager sqlManager;

        public AccountDatabase(SQLManager sqlManager)
        {
            this.sqlManager = sqlManager;
        }

        public Account Get(string login) => sqlManager.SelectSingle<Account>(@$"SELECT * FROM Accounts WHERE Login=""{login}""");
        public Account Get(int id) => sqlManager.SelectSingle<Account>(@$"SELECT * FROM Accounts WHERE ID=""{id}""");

        public void Create(Account account) => sqlManager.ExecuteNonQuery($"INSERT INTO Accounts(Login, Username, Password, IsDeveloper) " +
                                                                          @$"VALUES(""{account.Login}"", ""{account.Username}"", ""{account.Password}"", 0");
        
        public void Delete(string login) => sqlManager.ExecuteNonQuery(@$"DELETE FROM Accounts WHERE Login=""{login}""");
        public void Delete(int id) => sqlManager.ExecuteNonQuery($"DELETE FROM Accounts WHERE ID={id}");

        /// <summary>
        /// Updates info about account
        ///  - You can change only Username and Password
        ///  - If neither of those are changed, query will not be executed
        /// </summary>
        public void Update(Account account)
        {
            string query = "UPDATE Accounts SET ";

            if (!string.IsNullOrEmpty(account.Username))
                query += $@"Username = ""{account.Username}"",";

            if (!string.IsNullOrEmpty(account.Password))
                query += $@"Password = ""{account.Password}"",";

            if (query[^1] != ',')
                return;

            query = query.Remove(query.Length - 1);
            query += @$" WHERE Login = ""{account.Login}""";

            sqlManager.ExecuteNonQuery(query);
        }

        public bool DoesExist(string login) => Convert.ToBoolean(sqlManager.SelectSingle<int>(@$"SELECT COUNT(ID) FROM Accounts WHERE Login = ""{login}"""));
    }
}
