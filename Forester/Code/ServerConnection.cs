using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Forester.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Forester
{
    public static class ServerConnection
    {
        //Base link to api
        //public static string api = "http://***REMOVED***:5000";
        public static string api = "http://localhost:5000";

        public static HttpResponseMessage Get(string URI)
        {
            HttpClient client = new HttpClient();

            if (!string.IsNullOrWhiteSpace(Data.token.token))
                client.DefaultRequestHeaders.Add("token", Data.token.token);

            client.BaseAddress = new Uri(api);

            HttpResponseMessage message = client.GetAsync(URI).Result;

            return message;
        }

        public static HttpResponseMessage Put(string URI, object body)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(api);

            var data = new StringContent(JsonConverter.Serialize(body), Encoding.UTF8, "application/json");

            var response = client.PutAsync(URI, data).Result;

            return response;
        }

        public static HttpResponseMessage Post(string URI, object body)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(api);

            var data = new StringContent(JsonConverter.Serialize(body), Encoding.UTF8, "application/json");

            var response = client.PostAsync(URI, data).Result;

            return response;
        }

        public static Bitmap GetImage(string username)
        {
            var response = Get($"/api/Update/GetPicture?objectType=0&pictureType=0&name={username.Replace("#", "%23")}");

            Bitmap bitmap;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                bitmap = new Bitmap(response.Content.ReadAsStream());
            else
            {
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                bitmap = new Bitmap(assets.Open(new Uri("avares://Forester/Assets/defaultPP.png")));
            }

            return bitmap;
        }

        public static string Crypt(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
