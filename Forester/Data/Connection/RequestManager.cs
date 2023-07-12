using Forester.Models.API;
using Forester.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Data.Connection
{
    public class RequestManager : Manager
    {
        public RequestManager() : base()
        {
            client.BaseAddress = new Uri(BaseAddress);
        }

        // --- GET ---
        public async Task<APIMessage> Get<T>(string uri)
        {
            var response = await client.GetAsync(uri);
            return await CreateMessageFromResponse<T>(response);
        }

        // --- POST ---
        public async Task<APIMessage> Post<T>(string uri, string json)
        {
            var response = await RawPost(uri, json);
            return await CreateMessageFromResponse<T>(response);
        }

        public async Task<APIMessage> Post(string uri, string json)
        {
            var response = await RawPost(uri, json);
            return await CreateMessageFromResponse(response);
        }

        async Task<HttpResponseMessage> RawPost(string uri, string json)
        {
            return await client.PostAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"));
        }

        // --- PUT ---
        public async Task<APIMessage> Put(string uri, string json)
        {
            var response = await client.PutAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"));
            return await CreateMessageFromResponse(response);
        }
        
        async Task<APIMessage> CreateMessageFromResponse<T>(HttpResponseMessage message)
        {
            var content = await message.Content.ReadAsStringAsync();

            return new APIMessage()
            {
                StatusCode = message.StatusCode,
                Content = JsonManager.Deserialize<T>(content)!
            };
        }

        async Task<APIMessage> CreateMessageFromResponse(HttpResponseMessage message)
        {
            return new APIMessage()
            {
                StatusCode = message.StatusCode,
                Content = await message.Content.ReadAsStringAsync()
            };
        }
    }
}
