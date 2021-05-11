using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Forester
{
    public static class ServerConnection
    {
        static string api = "http://localhost:5000";

        public static string Get(string URI)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(api);

            HttpResponseMessage message = client.GetAsync(URI).Result;

            return message.Content.ReadAsStringAsync().Result;
        }

        public static string Post(string URI, object body)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(api);

            var response = client.PostAsJsonAsync(URI, body).Result;

            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
