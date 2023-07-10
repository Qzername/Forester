using Forester.Data;
using Forester.Models;
using Forester.Tools;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Forester.Services
{
    public class UserDataService : ReactiveObject
    {
        [Reactive] public Account CurrentAccount { get; private set; }
        [Reactive] public string Login { get; private set; }
        [Reactive] public string Username { get; private set; }

        //dependency injection
        AccountDatabase accountDatabase;

        public UserDataService(AccountDatabase accountDatabase)
        {
            this.accountDatabase = accountDatabase;
        }

        public void SetAccount(string login)
        {
            accountDatabase.Get(login);

            CurrentAccount = new Account()
            {
                Login = login,
            };

        }
    }
}
