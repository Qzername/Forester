using Forester.Data.Connection;
using Forester.Models.API;
using Forester.Models.Configurations;
using Forester.Services;
using Forester.Tools;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Forester.Data
{
    public class ApplicationDatabase : Database
    {
        protected override string APIprefix { get; } = "Applications/";

        public ApplicationDatabase(RequestManager requestManager, ErrorMessageService errorMessageService) : base(requestManager, errorMessageService) 
        {
        }

        public async Task<Application[]> Get()
        {
            var apiMessage = await RequestManager.Get<Application[]>(GenerateURI());

            if (apiMessage.StatusCode != HttpStatusCode.OK)
                SendError("Failed to get applications\nDo you have connection to internet?",
                    apiMessage.StatusCode,
                    "ApplicationDatabase -> GET() FAIL");

            return (Application[])apiMessage.Content;
        }

        public async Task<Application> GetById(int id)
        {
            var apiMessage = await RequestManager.Get<Application>(GenerateURI($"GetById?id={id}"));

            if (apiMessage.StatusCode != HttpStatusCode.OK)
              SendError("Failed to get application\nDo you have connection to internet?", 
                  apiMessage.StatusCode,
                  "ApplicationDatabase -> GETBYID(int id) FAIL");

            return (Application)apiMessage.Content;
        }

        public async Task<Application> GetByName(string name)
        {
            var apiMessage = await RequestManager.Get<Application>(GenerateURI($"GetByName?name={name}"));

            if (apiMessage.StatusCode != HttpStatusCode.OK)
                SendError("Failed to get application\nDo you have connection to internet?",
                    apiMessage.StatusCode,
                    "ApplicationDatabase -> GETBYNAME(string name) FAIL");

            return (Application)apiMessage.Content;
        }

        public async Task<bool> Create(Application application)
        {
            string json = JsonManager.Serialize(application);

            var apiMessage = await RequestManager.Post(GenerateURI(), json);

            return apiMessage.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> Update(string name, Application application)
        {
            string json = JsonManager.Serialize(application);

            var apiMessage = await RequestManager.Put(GenerateURI($"?name={name}"), json);

            return apiMessage.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> Delete(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// gets all applications that user develops
        /// </summary>
        /// <returns></returns>
        public async Task<Application[]> GetDeveloped()
        {
            var apiMessage = await RequestManager.Get<Application[]>(GenerateURI("GetDeveloped"));

            if (apiMessage.StatusCode != HttpStatusCode.OK)
                SendError("Failed to get applications\nDo you have connection to internet?",
                    apiMessage.StatusCode,
                    "ApplicationDatabase -> GETDEVELOPED() FAIL");

            return (Application[])apiMessage.Content;
        }

        public async Task<Account[]> GetApplicationAllowed(Application application)
        {
            string json = JsonManager.Serialize(application);

            var apiMessage = await RequestManager.Post<Account[]>(GenerateURI("GetApplicationAllowed"), json);

            if (apiMessage.StatusCode != HttpStatusCode.OK)
                SendError("Failed to get accounts\nDo you have connection to internet?",
                    apiMessage.StatusCode,
                    "ApplicationDatabase -> GETAPPLICAITONALLOWED(Application application) FAIL");

            return (Account[])apiMessage.Content;
        }

        public async Task<Account[]> GetApplicationDevelopers(Application application)
        {
            string json = JsonManager.Serialize(application);

            var apiMessage = await RequestManager.Post<Account[]>(GenerateURI("GetApplicationDevelopers"), json);

            if (apiMessage.StatusCode != HttpStatusCode.OK)
                SendError("Failed to get accounts\nDo you have connection to internet?",
                    apiMessage.StatusCode,
                    "ApplicationDatabase -> GETAPPLICAITONDEVELOPERS(Application application) FAIL");

            return (Account[])apiMessage.Content;
        }

        public async Task<bool> ChangePermission(string applicationName, string accountLogin, Permission permision)
        {
            string json = JsonManager.Serialize(new PermissionChangeData()
            {
                AccountLogin = accountLogin,
                ApplicationName = applicationName,
                Permission = permision
            });

            var apiMessage = await RequestManager.Post(GenerateURI("ChangePermission"), json);

            return apiMessage.StatusCode == HttpStatusCode.OK;
        }
    }
}