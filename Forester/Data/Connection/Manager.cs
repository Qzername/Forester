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
        protected const string BaseAddress = "http://***REMOVED***:5000/";

        protected HttpClient client;

        public Manager()
        {
            client = new HttpClient();
        }
        public void SetAuthorization(string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
