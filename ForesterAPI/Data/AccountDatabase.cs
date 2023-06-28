using ForesterAPI.Data.Connection;
using ForesterAPI.Models;
using Microsoft.Win32;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace ForesterAPI.Data
{
    public class AccountDatabase
    {
        SQLManager sqlManager;

        public AccountDatabase(SQLManager SQLManager)
        {
            this.sqlManager = SQLManager;
        }

        public Account Get(string Login) => sqlManager.SelectSingle<Account>(@$"SELECT * FROM Accounts WHERE Login=""{Login}""");
        public Account Get(int ID) => sqlManager.SelectSingle<Account>(@$"SELECT * FROM Accounts WHERE ID=""{ID}""");

        /// <summary>
        /// Creates new account
        /// - Does not verify if data is correct
        /// </summary>
        public void Create(Account Account) => sqlManager.ExecuteNonQuery($"INSERT INTO Accounts(Login, Username, Password, IsDeveloper) " +
                                                                          @$"VALUES(""{Account.Login}"", ""{Account.Username}"", ""{Account.Password}"", 0");
        
        public void Delete(string Login) => sqlManager.ExecuteNonQuery(@$"DELETE FROM Accounts WHERE Login=""{Login}""");
        public void Delete(int ID) => sqlManager.ExecuteNonQuery($"DELETE FROM Accounts WHERE ID={ID}");

        /// <summary>
        /// Updates info about account
        ///  - You can change only Username and Password
        ///  - If neither of those are changed, query will not be executed
        ///  - Does not verify if data is correct
        /// </summary>
        public void Update(Account Account)
        {
            string query = "UPDATE Accounts SET ";

            if (!string.IsNullOrEmpty(Account.Username))
                query += $@"Username = ""{Account.Username}"",";

            if (!string.IsNullOrEmpty(Account.Password))
                query += $@"Password = ""{Account.Password}"",";

            if (query[^1] != ',')
                return;

            query = query.Remove(query.Length - 1);
            query += @$" WHERE Login = ""{Account.Login}""";

            sqlManager.ExecuteNonQuery(query);
        }

        public bool DoesExist(string Login) => Convert.ToBoolean(sqlManager.SelectSingle<int>(@$"SELECT COUNT(ID) FROM Accounts WHERE Login = ""{Login}"""));
    }
}
