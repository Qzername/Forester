using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Data.Connection
{
    public abstract class Manager
    {
        protected readonly string BaseAddress;

        protected HttpClient client;

        public Manager(string serverIp)
        {
            BaseAddress = $"http://{serverIp}/";

            client = new HttpClient();
        }
        public void SetAuthorization(string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
