using Avalonia;
using Avalonia.Controls;
using AvaMedia = Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Forester.Models.API;
using RestSharp;
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
using System.Globalization;
using Forester.Code.AppData;

namespace Forester
{
    public static class ServerConnection
    {
        //Base link to api
        public static string api = "http://***REMOVED***:5000";
        //public static string api = "http://localhost:5000";

        static HttpClient client;

        static ServerConnection()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(api);
        }

        public static void SetToken(string token)
        {
            if(!client.DefaultRequestHeaders.Contains("token"))
                client.DefaultRequestHeaders.Add("token", token);
        }

        public static HttpResponseMessage Get(string URI) => client.GetAsync(URI).Result;

        public static HttpResponseMessage Put(string URI, object body)
        {
            var data = new StringContent(JsonConverter.Serialize(body), Encoding.UTF8, "application/json");
            return client.PutAsync(URI, data).Result;
        }

        public static HttpResponseMessage Post(string URI, object body)
        {
            var data = new StringContent(JsonConverter.Serialize(body), Encoding.UTF8, "application/json");
            return client.PostAsync(URI, data).Result;
        }

        /// <summary>
        /// Wzięcie zdjęcia z serwera
        /// </summary>
        /// <param name="username">Nazwa aplikacji/konta użytkownika</param>
        /// <param name="objectType">Czego profilowe chcesz, aplikacji czy użytkownika; 0 = użytkownik, 1 = aplikacja</param>
        /// <param name="pictureType">Jaki rodzaj zdjęcia chcesz; 0 = profilowe, 1 = tło</param>
        /// <returns></returns>
        public static (Bitmap,bool) GetImage(string username, int objectType, int pictureType)
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

            return (bitmap, response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        public static async void PostImage(string username, int objectType, int pictureType, Bitmap image)
        {
            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                image.Save(stream);
                bytes = stream.ToArray();
            }

            var response = Upload(bytes, $"/api/Update/UpdatePicture?objectType={objectType}&pictureType={pictureType}&name={username.Replace("#", "%23")}");
        }

        public static HttpResponseMessage Upload(byte[] file, string uri)
        {
            HttpClient client = new HttpClient();
            
            if (!string.IsNullOrWhiteSpace(Data.token.token))
                client.DefaultRequestHeaders.Add("token", Data.token.token);

            var content = new MultipartFormDataContent();

            content.Add(new StreamContent(new MemoryStream(file)), "file", "FILE");

            return client.PutAsync(api + uri, content).Result;
        }

        public static Task Upload(ref WebClient client, string URI, string filePath)
        {
            client.Headers.Add("token", Data.token.token);
            client.UploadFileTaskAsync(new Uri(api + URI), "POST", filePath);
            return Task.CompletedTask;
        }

        public static Task Upload(string filePath, string uri)
        {
            HttpClient httpClient = new HttpClient();

            if (!string.IsNullOrWhiteSpace(Data.token.token))
                httpClient.DefaultRequestHeaders.Add("token", Data.token.token);

            httpClient.Timeout = new TimeSpan(7, 0, 0, 0);

            using (var multipartFormContent = new MultipartFormDataContent())
            {
                //Load the file and set the file's Content-Type header
                var fileStreamContent = new StreamContent(File.OpenRead(filePath));
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                //Add the file
                multipartFormContent.Add(fileStreamContent, name: "file", fileName: "app.zip");

                //Send it
                var response = httpClient.PostAsync(api+uri, multipartFormContent);

                System.Diagnostics.Debug.WriteLine(response.Result.Content.ReadAsStringAsync().Result);

                response.Result.EnsureSuccessStatusCode();
            }

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
        static Bitmap GenerateGradient()
        {
            int height = 400, width = 1000;

            string hexString = Data.config.theme.colorThird;
            byte r, g, b;

            r = byte.Parse(hexString.Substring(0, 2), NumberStyles.AllowHexSpecifier);
            g = byte.Parse(hexString.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            b = byte.Parse(hexString.Substring(4, 2), NumberStyles.AllowHexSpecifier);

            var color = Drawing.Color.FromArgb(r,g,b);

            using (Drawing.Bitmap bitmap = new Drawing.Bitmap(width, height))
            using (Drawing.Graphics graphics = Drawing.Graphics.FromImage(bitmap))
            using (LinearGradientBrush brush = new LinearGradientBrush(new Drawing.Point(0, 0), new Drawing.Point(height, width), Drawing.Color.Black, color))
            {
                brush.SetSigmaBellShape(0.8f);
                graphics.FillRectangle(brush, new Drawing.Rectangle(0, 0, width, height));

                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;

                    return new Bitmap(memory);
                }
            }
        }
    }
}