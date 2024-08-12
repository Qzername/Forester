using Avalonia.Controls;
using Forester.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Forester.Data.Connection
{
    public class FileTransferManager : Manager
    {
        public FileTransferManager(string ip) : base(ip)
        {
            client.BaseAddress = new Uri(BaseAddress);
            client.Timeout = new TimeSpan(24, 0, 0);
        }

        public async Task Download(string uri, string filepath, IProgress<int> progress, string doNotIncludeJson = "{}") 
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(doNotIncludeJson, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                
            var contentLength = response.Content.Headers.ContentLength;

            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var fileStream = new FileStream(filepath, FileMode.Create))
            {
                byte[] buffer = new byte[8192];
                long totalBytesRead = 0L;
                int bytesRead = 0;
                double currentProgress = 0.0;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);

                    totalBytesRead += bytesRead;

                    currentProgress = (double)totalBytesRead / contentLength!.Value * 100;

                    progress.Report(Convert.ToInt32(currentProgress));
                }
            }
        }

        //for the images
        public async Task<byte[]> Download(string uri)
        {
            var response = await client.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
                return [];

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task Upload(string uri, Stream file, IProgress<int>? progress = null)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(file), "file", "FILE");

            bool keepTracking = true; //to start and stop the tracking thread
            
            if(progress is not null)
                new Task(new Action(() => { ProgressTracking(file, ref keepTracking, progress); })).Start();
            
            var result = await client.PostAsync(uri, content);
            
            keepTracking = false; //stops the tracking thread
        }

        void ProgressTracking(Stream streamToTrack, ref bool keepTracking, IProgress<int> progress)
        {
            int prevPos = -1;
            while (keepTracking)
            {
                int pos = (int)Math.Round(100 * (streamToTrack.Position / (double)streamToTrack.Length));

                if (pos != prevPos)
                    progress.Report(pos);

                prevPos = pos;

                Thread.Sleep(100); //update every 100ms
            }
        }
    }
}
