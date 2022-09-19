using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Forester.Models.API;

namespace Forester.ViewModels.App.Developer.ManageApp
{
    internal class UploadNewVersion : ViewModelBase
    {
        string _newVersion;
        public string newVersion
        {
            get => _newVersion;
            set => this.RaiseAndSetIfChanged(ref _newVersion, value);
        }

        string _newVersionCurrentPath;
        public string newVersionCurrentPath
        {
            get => _newVersionCurrentPath;
            set => this.RaiseAndSetIfChanged(ref _newVersionCurrentPath, value);
        }

        float _versionPBValue;
        public float versionPBValue
        {
            get => _versionPBValue;
            set => this.RaiseAndSetIfChanged(ref _versionPBValue, value);
        }

        string _errorNewVersion;
        public string errorNewVersion
        {
            get => _errorNewVersion;
            set => this.RaiseAndSetIfChanged(ref _errorNewVersion, value);
        }

        string pathToFile;

        public UploadNewVersion()
        {
            newVersionCurrentPath = "Current path: None";
        }

        public async void SelectPath()
        {
            var dialog = new OpenFolderDialog();

            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                string? result = await dialog.ShowAsync(desktop.MainWindow);

                if (result is null)
                    return;

                pathToFile = result;
                newVersionCurrentPath = "Current path: " + pathToFile;
            }
        }

        public void SubmitNewVersion()
        {
            errorNewVersion = "";
            versionPBValue = 0f;

            if (pathToFile == string.Empty)
            {
                errorNewVersion = "Path cannot be empty";
                return;
            }

            Thread uploadThread = new Thread(UploadThread);
            uploadThread.Start();
        }

        public async void UploadThread()
        {
            errorNewVersion = "Preparing...";

            string path = AppFileManager.PrepareApp(pathToFile);

            WebClient client = new WebClient();
            client.UploadProgressChanged += Client_UploadProgressChanged;
            client.UploadFileCompleted += Client_UploadFileCompleted;

            errorNewVersion = "Uploading... Forester is freezed until upload is completed...";

            /*await ServerConnection.Upload(path, "/api/Download/Upload?name=" + ManageAppViewModel.Current.currentApp.name);
            Client_UploadFileCompleted(null, null); więcej info w ServerConnection.Upload*/

            await ServerConnection.Upload(ref client, "/api/Download/Upload?name=" + ManageAppViewModel.Current.currentApp.name, path);
        }

        private void Client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            errorNewVersion = "Uploading completed.";
            AppFileManager.Clear();

            DeveloperViewModel.Current.RefreshList();

            BasicInformation.Current.currentVersion = "Current version: " + newVersion;

            Application update = new Application()
            {
                ID = 0,
                name = "",
                quickDescription = "",
                description = "",
                isPrivate = "",
                mainDeveloper = 0,
                downloadNumber = 0,
                version = newVersion
            };

            ServerConnection.Put("/api/Applications/Update?name=" + ManageAppViewModel.Current.currentApp.name, update);

            newVersion = string.Empty;
            newVersionCurrentPath = "Current path: None";

            DeveloperViewModel.Current.RefreshList();
        }

        private void Client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            float progress = (float)e.BytesSent / new FileInfo("./tempApp.zip").Length;

            if (progress * 100 < versionPBValue)
                return;

            versionPBValue = (float)Math.Round(progress * 100, 2);
        }
    }
}
