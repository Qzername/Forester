using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Forester.Models.API;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Drawing = System.Drawing;

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

            if (!string.IsNullOrWhiteSpace(Data.token.token))
                client.DefaultRequestHeaders.Add("token", Data.token.token);

            var data = new StringContent(JsonConverter.Serialize(body), Encoding.UTF8, "application/json");

            var response = client.PutAsync(URI, data).Result;

            return response;
        }

        public static HttpResponseMessage Post(string URI, object body)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(api);

            if (!string.IsNullOrWhiteSpace(Data.token.token))
                client.DefaultRequestHeaders.Add("token", Data.token.token);

            var data = new StringContent(JsonConverter.Serialize(body), Encoding.UTF8, "application/json");

            var response = client.PostAsync(URI, data).Result;

            return response;
        }

        public static Bitmap GetImage(string username, int objectType, int pictureType)
        {
            var response = Get($"/api/Update/GetPicture?objectType={objectType}&pictureType={pictureType}&name={username.Replace("#", "%23")}");

            Bitmap bitmap;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                bitmap = new Bitmap(response.Content.ReadAsStream());
            else
            {
                if(pictureType == 0)
                {
                    var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                    bitmap = new Bitmap(assets.Open(new Uri("avares://Forester/Assets/defaultPP.png")));
                }
                else
                    bitmap = GenerateGradient();
            }

            return bitmap;
        }

        static Bitmap GenerateGradient()
        {
            int height = 400, width = 800;

            using (Drawing.Bitmap bitmap = new Drawing.Bitmap(width, height))
            using (Drawing.Graphics graphics = Drawing.Graphics.FromImage(bitmap))
            using (LinearGradientBrush brush = new LinearGradientBrush(new Drawing.Point(0, 0), new Drawing.Point(height, width), Drawing.Color.Black, Drawing.Color.Green))
            {
                brush.SetSigmaBellShape(0.7f);
                graphics.FillRectangle(brush, new Drawing.Rectangle(0, 0, width, height));

                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;

                    return new Bitmap(memory);
                }
            }
        }

        public static Task Upload(ref WebClient client, string URI, string filePath)
        {
            client.Headers.Add("token", Data.token.token);
            client.UploadFileTaskAsync(new Uri(api + URI), "POST", filePath);
            return Task.CompletedTask;
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