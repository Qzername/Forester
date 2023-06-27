using ForesterAPI.Data.Connection;
using ForesterAPI.Models;

namespace ForesterAPI.Data
{
    public class AccountDatabase
    {
        SQLManager sqlManager;

        public AccountDatabase(SQLManager SQLManager)
        {
            this.sqlManager = SQLManager;
        }

        public Account GetAccount(string Login) => sqlManager.SelectSingle<Account>($"SELECT * FROM Accounts WHERE Login=\"{Login}\"");
        public Account GetAccount(int ID) => sqlManager.SelectSingle<Account>($"SELECT * FROM Accounts WHERE ID=\"{ID}\"");
    }
}
