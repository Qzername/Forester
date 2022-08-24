using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Forester.ViewModels
{
    public class DownloadUpdateViewModel : ViewModelBase
    {
        float _progress;
        public float progress
        {
            get => _progress;
            set => this.RaiseAndSetIfChanged(ref _progress, value);
        }

        HttpClientDownloadWithProgress client;
        Thread downloadThread;

        public DownloadUpdateViewModel()
        {
            client = new HttpClientDownloadWithProgress();
        }

        public async void Download()
        {
            downloadThread = new Thread(DownloadThread);

            downloadThread.Start();
        }

        async void DownloadThread()
        {
            var client = new HttpClientDownloadWithProgress();

            client.ProgressChanged += Client_ProgressChanged;
            client.DownloadFinished += Client_DownloadFinished;

            Dictionary<string,string> config = JsonConverter.Deserialize<Dictionary<string, string>>(FileReader.ReadText("./config.json"));
            await client.DownloadFileFromHttpResponseMessage(ServerConnection.Post("/api/Update/Forester/Download", config));
        }

        private void Client_DownloadFinished()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                downloadThread = null;
                client = null;

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "./Updater/Updater.exe"
                    }
                };

                process.Start();

                desktop.Shutdown();
            }
        }

        private void Client_ProgressChanged(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage)
        {
            progress = Convert.ToSingle(progressPercentage);
        }
    }
}
