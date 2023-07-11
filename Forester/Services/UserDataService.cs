using Forester.Data;
using Forester.Models.API;
using Forester.Tools;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Threading.Tasks;

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

        public async Task SetAccount(string login)
        {
            CurrentAccount = await accountDatabase.Get(login);
        }
    }
}
