using Forester.Data.Connection;
using Forester.Models;
using Forester.Tools;
using Splat;
using System.Threading.Tasks;

namespace Forester.Data
{
    public class AccountDatabase
    {
        const string APIprefix = "Accounts/";

        RequestManager requestManager;

        public AccountDatabase(RequestManager requestManager) 
        {
            this.requestManager = requestManager;
        }

        public async Task Register(Account account)
        {
            string json = JsonManager.Serialize(account);
            await requestManager.Post(GenerateURI("Register"), json);
        }

        public async Task<string> Login(Account account)
        {
            string json = JsonManager.Serialize(account);
            var response = await requestManager.Post(GenerateURI("Login"), json);

            return await response.Content.ReadAsStringAsync();
        }

        string GenerateURI(string action) => APIprefix + action;
    }
}
