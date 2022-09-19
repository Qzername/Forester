using Avalonia.Media.Imaging;
using Forester.Models;
using Forester.ViewModels.Popup.Library;
using ReactiveUI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forester.ViewModels.App.Library
{
    public class AppViewModel : ViewModelBase
    {
        public static AppViewModel Current;

        Bitmap _backgroundPicture; 
        public Bitmap backgroundPicture
        {
            get => _backgroundPicture;
            set => this.RaiseAndSetIfChanged(ref _backgroundPicture, value);
        }

        string _appName;
        public string appName
        {
            get=> _appName;
            set => this.RaiseAndSetIfChanged(ref _appName, value);
        }

        string _description;
        public string description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        float _progress;
        public float progress
        { 
            get => _progress;
            set=> this.RaiseAndSetIfChanged(ref _progress, value);
        }

        bool _canDownload;
        public bool canDownload
        {
            get => _canDownload;
            set => this.RaiseAndSetIfChanged(ref _canDownload, value);
        }

        bool isDownloading;

        LibraryElement currentApp;
        HttpClientDownloadWithProgress progressDownload;

        public AppViewModel()
        {
            Current = this;

            progressDownload = new HttpClientDownloadWithProgress();
            progressDownload.ProgressChanged += ProgressDownload_ProgressChanged;
            progressDownload.DownloadFinished += ProgressDownload_DownloadFinished;

            isDownloading = false;
        }

        private void ProgressDownload_DownloadFinished()
        {
            AppFileManager.AppDownloaded(currentApp.appConfig,currentApp.app.name);
            isDownloading = false;
            SetApp(currentApp);
        }

        private void ProgressDownload_ProgressChanged(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage)
        {
            this.progress = System.Convert.ToSingle(progressPercentage);
        }

        public void SetApp(LibraryElement app)
        {
            if (isDownloading)
                return;

            currentApp = app;

            appName = currentApp.app.name;
            backgroundPicture = currentApp.backgroundPicture;
            description = currentApp.app.description;

            canDownload = !AppFileManager.IsAppDownloaded(app.appConfig, appName);
        }

        public void Update() => DownloadApp();

        public void Download()
        {
            PopupConfig config = new PopupConfig()
            {
                content = new DownloadSettingsViewModel(),
                height = 300,
                width = 600,
                margin = new Avalonia.Thickness(0, 20, 0, 0)
            };

            MainWindowViewModel.Current.CreatePopup(config);
        }

        public void ConfirmedDownload(bool isDefaultPath, string path, bool deleteNotNecessary)
        {
            currentApp.appConfig = new AppConfig()
            {
                id = currentApp.appConfig.id,
                isDefaultPath = isDefaultPath,
                path = path +(path.EndsWith("/") ? "" : "/")+ appName + "/",
                deleteNotNecessaryFiles = deleteNotNecessary,
            };

            canDownload = false;
            LibraryViewModel.Current.SetConfig(currentApp.appConfig);

            DownloadApp();
        }

        async void DownloadApp()
        {
            Dictionary<string, string> dict = AppFileManager.ReadAppConfig(currentApp.appConfig,currentApp.app.name);

            isDownloading = true;

            using (var response = await ServerConnection.AsyncPost("/api/Download/Load?name=" + currentApp.app.name, dict))
                await progressDownload.DownloadFileFromHttpResponseMessage(response);
        }

        public void Delete() 
        { 
            AppFileManager.AppDelete(currentApp.appConfig, currentApp.app.name);
            canDownload = true;
        }

        public void DeleteFromLibrary()
        {
            Delete();
            LibraryViewModel.Current.DeleteApp(currentApp.app.name);
            LibraryViewModel.Current.content = new DefaultAppViewModel();
        }
    }
}