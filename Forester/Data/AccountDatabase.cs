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

        public async Task<APIMessage> Register(Account account)
        {
            string json = JsonManager.Serialize(account);
            return await requestManager.Post(GenerateURI("Register"), json);
        }

        public async Task<APIMessage> Login(Account account)
        {
            string json = JsonManager.Serialize(account);
            return await requestManager.Post(GenerateURI("Login"), json);
        }

        string GenerateURI(string action) => APIprefix + action;
    }
}
