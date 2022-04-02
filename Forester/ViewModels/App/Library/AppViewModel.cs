using Avalonia.Media.Imaging;
using Forester.Models;
using ReactiveUI;
using System.Collections.Generic;

namespace Forester.ViewModels.App.Library
{
    public class AppViewModel : ViewModelBase
    {
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
            AppFileManager.AppDownloaded(currentApp.app.name);
        }

        private void ProgressDownload_ProgressChanged(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage)
        {
            this.progress = System.Convert.ToSingle(progressPercentage);
        }

        public void SetApp(LibraryElement app)
        {
            currentApp = app;

            appName = currentApp.app.name;
            backgroundPicture = currentApp.backgroundPicture;
            description = currentApp.app.description;
        }

        public void Run()
        {
            
        }

        public async void Download()
        {
            Dictionary<string, string> dict = AppFileManager.ReadAppConfig(currentApp.app.name);
            await progressDownload.DownloadFileFromHttpResponseMessage(ServerConnection.Post("/api/Download/Load?name=" + currentApp.app.name, dict));
        }

        public void Delete() => AppFileManager.AppDelete(currentApp.app.name);

        public void DeleteFromLibrary()
        {
            Delete();
            libraryVM.DeleteApp(currentApp.app.name);
            libraryVM.content = new DefaultAppViewModel();
        }
    }
}