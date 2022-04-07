using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels
{
    public class DownloadUpdateViewModel : ViewModelBase
    {
        public SolidColorBrush first
        {
            get => MainWindowViewModel.first;
        }

        public SolidColorBrush second
        {
            get => MainWindowViewModel.second;
        }

        public SolidColorBrush third
        {
            get => MainWindowViewModel.third;
        }

        float _progress;
        public float progress
        {
            get => _progress;
            set => this.RaiseAndSetIfChanged(ref _progress, value);
        }

        HttpClientDownloadWithProgress client;

        public async void Download()
        {
            client = new HttpClientDownloadWithProgress();

            client.ProgressChanged += Client_ProgressChanged;
            client.DownloadFinished += Client_DownloadFinished;

            client.DownloadFileFromHttpResponseMessage(ServerConnection.Post("/api/Update/Forester/Download",new Dictionary<string,string>()));
        }

        private void Client_DownloadFinished()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
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
