using Forester.Data.Connection;
using Forester.Models.API;
using Forester.Models.Configurations;
using Forester.Services;
using Forester.Tools;
using Splat;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Forester.Data
{
    public class AccountDatabase : Database
    {
        protected override string APIprefix { get; } = "Accounts/";

        public AccountDatabase(RequestManager requestManager, FileTransferManager fileTransferManager, ErrorMessageService errorMessageService) : base(requestManager, fileTransferManager, errorMessageService)
        {
        }

        /// <summary>
        /// returns if request was ok
        /// </summary>
        public async Task<bool> Register(Account account)
        {
            string json = JsonManager.Serialize(account);

            var apiMessage = await RequestManager.Post(GenerateURI("Register"), json);

            return apiMessage.StatusCode == HttpStatusCode.OK;
        }

        /// <summary>
        /// returns if request is ok if yes adds token to ServerConnection
        /// </summary>
        public async Task<bool> Login(Account account)
        {
            string json = JsonManager.Serialize(account);

            var apiMessage = await RequestManager.Post<APIToken>(GenerateURI("Login"), json);

            if (apiMessage.StatusCode == HttpStatusCode.OK)
            {
                string token = ((APIToken)apiMessage.Content).Token;

                RequestManager.SetAuthorization(token);
                FileTransferManager.SetAuthorization(token);

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// returns if request is ok
        /// </summary>
        public async Task Update(Account account)
        {
            string json = JsonManager.Serialize(account);

            var apiMessage = await RequestManager.Put(GenerateURI("Update"), json);
            
            if(apiMessage.StatusCode != HttpStatusCode.OK)
                SendError("Failed to update account\nDo you have connection to internet?",
                    apiMessage.StatusCode,
                    "AccountDatabase -> UPDATE(Account account) FAIL");
        }

        public async Task<Account> Get(string login)
        {
            var apiMessage = await RequestManager.Get<Account>(GenerateURI($"GetByLogin?login={login}"));

            if (apiMessage.StatusCode != HttpStatusCode.OK)
                SendError("Failed to get user by his Login\nDo you have connection to internet?",
                    apiMessage.StatusCode,
                    "AccountDatabase -> GET(string login) FAIL");

            return (Account)apiMessage.Content;
        }

        public async Task<Account> Get(int id)
        {
            var apiMessage = await RequestManager.Get<Account>(GenerateURI($"GetById?id={id}"));

            if(apiMessage.StatusCode != HttpStatusCode.OK)
                SendError("Failed to get user by his id\nDo you have connection to internet?",
                    apiMessage.StatusCode,
                    "AccountDatabase -> GET(int id) FAIL");

            return (Account)apiMessage.Content;
        }

        public async Task<bool> Delete()
        {
            throw new NotImplementedException();
        }
    }
}
