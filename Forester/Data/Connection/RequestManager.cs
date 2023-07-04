using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public async Task<string> Get(string uri)
        {
            var response = await client.GetAsync(uri);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task Post(string uri, string json)
        {
            await client.PostAsync(uri, new StringContent(json));
        }
    }
}
