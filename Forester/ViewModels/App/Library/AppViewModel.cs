using Avalonia.Media.Imaging;
using Forester.Models;
using Forester.ViewModels.Popup.Library;
using ReactiveUI;
using System.Collections.Generic;

namespace Forester.ViewModels.App.Library
{
    public class AppViewModel : ViewModelBase
    {
        Bitmap backgroundPicture; 
        public Bitmap BackgroundPicture
        {
            get => backgroundPicture;
            set => this.RaiseAndSetIfChanged(ref backgroundPicture, value);
        }

        string appName;
        public string AppName
        {
            get=> appName;
            set => this.RaiseAndSetIfChanged(ref appName, value);
        }

        string description;
        public string Description
        {
            get => description;
            set => this.RaiseAndSetIfChanged(ref description, value);
        }

        float progress;
        public float Progress
        { 
            get => progress;
            set=> this.RaiseAndSetIfChanged(ref progress, value);
        }

        bool canDownload;
        public bool CanDownload
        {
            get => canDownload;
            set => this.RaiseAndSetIfChanged(ref canDownload, value);
        }

        LibraryElement currentApp;
        LibraryViewModel libraryVM;
        HttpClientDownloadWithProgress progressDownload;

        public AppViewModel(LibraryViewModel libraryVM)
        {
            this.libraryVM = libraryVM;
            progressDownload = new HttpClientDownloadWithProgress();
            progressDownload.ProgressChanged += ProgressDownload_ProgressChanged;
            progressDownload.DownloadFinished += ProgressDownload_DownloadFinished;
        }

        private void ProgressDownload_DownloadFinished()
        {
            AppFileManager.AppDownloaded(currentApp.appConfig,currentApp.app.name);
        }

        private void ProgressDownload_ProgressChanged(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage)
        {
            this.Progress = System.Convert.ToSingle(progressPercentage);
        }

        public void SetApp(LibraryElement app)
        {
            currentApp = app;

            AppName = currentApp.app.name;
            BackgroundPicture = currentApp.backgroundPicture;
            Description = currentApp.app.description;

            CanDownload = !AppFileManager.IsAppDownloaded(app.appConfig, AppName);
        }

        public void Update() => DownloadApp();

        public void Download()
        {
            PopupConfig config = new PopupConfig()
            {
                content = new DownloadSettingsViewModel(this),
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
                path = path,
                deleteNotNecessaryFiles = deleteNotNecessary,
            };

            CanDownload = false;
            libraryVM.SetConfig(currentApp.appConfig);

            DownloadApp();
        }

        async void DownloadApp()
        {
            Dictionary<string, string> dict = AppFileManager.ReadAppConfig(currentApp.appConfig,currentApp.app.name);
            await progressDownload.DownloadFileFromHttpResponseMessage(ServerConnection.Post("/api/Download/Load?name=" + currentApp.app.name, dict));
        }

        public void Delete() 
        { 
            AppFileManager.AppDelete(currentApp.appConfig, currentApp.app.name);
            CanDownload = true;
        }

        public void DeleteFromLibrary()
        {
            Delete();
            libraryVM.DeleteApp(currentApp.app.name);
            libraryVM.Content = new DefaultAppViewModel();
        }
    }
}