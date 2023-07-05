using Forester.Models;
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
        const string BaseAddress = "http://localhost:5000/";

        HttpClient client;

        public RequestManager()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(BaseAddress);
        }

        public async Task<APIMessage> Get(string uri)
        {
            var response = await client.GetAsync(uri);
            return await CreateMessageFromResponse(response);
        }

        public async Task<APIMessage> Post(string uri, string json)
        {
            var response = await client.PostAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"));
            return await CreateMessageFromResponse(response);
        }

        async Task<APIMessage> CreateMessageFromResponse(HttpResponseMessage message)
        {
            var content = await message.Content.ReadAsStringAsync();

            return new APIMessage()
            {
                StatusCode = message.StatusCode,
                Message = content
            };
        }
    }
}
