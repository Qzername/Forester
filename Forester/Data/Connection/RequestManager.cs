using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Data.Connection
{
    public class RequestManager
    {
        const string BaseAddress = "http://localhost:5000/api/";

        HttpClient client;

        public RequestManager()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(BaseAddress);
        }

        public async Task<HttpResponseMessage> Get(string uri)
        {
            return await client.GetAsync(uri);
        }

        public async Task<HttpResponseMessage> Post(string uri, string json)
        {
            return await client.PostAsync(uri, new StringContent(json));
        }
    }
}
