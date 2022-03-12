using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Forester;
using Forester.Models.API;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Forester.ViewModels.App.Developer
{
    public class ManageAppViewModel : ViewModelBase
    {
        #region Basic App Information
        public string _appName, _quickDescription, _description, _currentVersion;

        public string appName
        {
            get => _appName;
            set => this.RaiseAndSetIfChanged(ref _appName, value);
        }
        public string quickDescription
        {
            get => _quickDescription;
            set => this.RaiseAndSetIfChanged(ref _quickDescription, value);
        }
        public string description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }
        public string currentVersion
        {
            get => _currentVersion;
            set => this.RaiseAndSetIfChanged(ref _currentVersion, value);
        }

        Bitmap _profilePicture;
        public Bitmap profilePicture
        {
            get => _profilePicture;
            set => this.RaiseAndSetIfChanged(ref _profilePicture, value);
        }

        Bitmap _backgroundPicture;
        public Bitmap backgroundPicture
        {
            get => _backgroundPicture;
            set => this.RaiseAndSetIfChanged(ref _backgroundPicture, value);
        }

        Bitmap tempProfilePicture, tempBackgroundPicture;

        bool _makeChanges;
        public bool makeChanges
        {
            get => _makeChanges;
            set => this.RaiseAndSetIfChanged(ref _makeChanges, value);
        }

        string _switchChangesButtonContent;
        public string switchChangesButtonContent
        {
            get => _switchChangesButtonContent;
            set => this.RaiseAndSetIfChanged(ref _switchChangesButtonContent, value);
        }

        string _errorSwitchChanges;
        public string errorSwitchChanges
        {
            get => _errorSwitchChanges;
            set => this.RaiseAndSetIfChanged(ref _errorSwitchChanges, value);
        }

        #endregion

        #region Upload New Version
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
        #endregion

        Application currentApp;
        DeveloperViewModel developerVM;

        public ManageAppViewModel(DeveloperViewModel developerVM)
        {
            newVersionCurrentPath = "Current path: None";
            this.developerVM = developerVM;
        }

        #region Basic app information 
        public void SwitchChanges()
        {
            if (makeChanges)
            {
                switchChangesButtonContent = "Make changes";
                SetBasicInformation(currentApp);
            }
            else
                switchChangesButtonContent = "Cancel changes";

            makeChanges = !makeChanges;
        }

        public async void UploadNewProfilePicture()
        {
            var photo = await ReadPhoto();

            if (photo is null)
                return;

            if (tempProfilePicture is null)
                tempProfilePicture = profilePicture;

            profilePicture = photo;
        }

        public async void UploadNewBackgroundPicture()
        {
            var photo = await ReadPhoto();

           if (photo is null)
                return;

            if(tempBackgroundPicture is null)
                tempBackgroundPicture = backgroundPicture;

            backgroundPicture = photo;
        }

        async Task<Bitmap> ReadPhoto()
        {
            var dialog = new OpenFileDialog();

            string pathToFile = string.Empty;

            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                string[]? result = await dialog.ShowAsync(desktop.MainWindow);

                if (result is null)
                    return null;

                pathToFile = result[0];
            }

            return FileReader.ReadPhoto(pathToFile);
        }

        public void SubmitChanges()
        {
            errorSwitchChanges = "";

            UpdatePhotos();
            UpdateBasicInformation();

            developerVM.RefreshList();

            var response = ServerConnection.Get("/api/Applications/Get?name=" + appName);
            SetApp(JsonConverter.Deserialize<Application[]>(response.Content.ReadAsStringAsync().Result)[0]);
        }

        void UpdateBasicInformation()
        {
            Application update = new Application()
            {
                ID = 0,
                name = "",
                quickDescription = "",
                description = "",
                isPrivate = "",
                mainDeveloper = 0,
                downloadNumber = 0,
                version = ""
            };

            if (appName != currentApp.name)
                update.name = appName;

            if (quickDescription != currentApp.quickDescription)
                update.quickDescription = quickDescription;

            if (description != currentApp.description)
                update.description = description;

            var response = ServerConnection.Put("/api/Applications/Update?name=" + currentApp.name, update);

            if (!response.IsSuccessStatusCode)
            {
                //kod 499 - nazwa jest już zajęta
                errorSwitchChanges = "Name is taken.";
                return;
            }
        }

        void UpdatePhotos()
        {
            //zdjęcia
            if (tempProfilePicture is not null)
                ServerConnection.PostImage(currentApp.name, 1, 0, profilePicture);

            if (tempBackgroundPicture is not null)
                ServerConnection.PostImage(currentApp.name, 1, 1, backgroundPicture);
        }

        void SetBasicInformation(Application app)
        {
            errorSwitchChanges = "";

            if (tempProfilePicture is null)
                profilePicture = ServerConnection.GetImage(app.name, 1, 0);
            else
                tempProfilePicture = null;

            if(tempBackgroundPicture is null)
                backgroundPicture = ServerConnection.GetImage(app.name, 1, 1);
            else
                tempBackgroundPicture = null;

            appName = app.name;
            quickDescription = app.quickDescription;
            description = app.description;

            currentVersion = "Current version: " + app.version;
        }
        #endregion

        #region Upload New Version
        public void SetApp(Application app)
        {
            switchChangesButtonContent = "Make changes";
            makeChanges = false;
            currentApp = app;

            SetBasicInformation(app);
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

        public void UploadThread()
        {
            string path = AppFileManager.PrepareApp(pathToFile);

            WebClient client = new WebClient();
            client.UploadProgressChanged += Client_UploadProgressChanged;
            client.UploadFileCompleted += Client_UploadFileCompleted;

            errorNewVersion = "Uploading...";

            ServerConnection.Upload(ref client, "/api/Download/Upload?name=" + currentApp.name, path);
        }

        private void Client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            errorNewVersion = "Uploading completed.";
            AppFileManager.Clear();

            developerVM.RefreshList();

            currentVersion = "Current version: " + newVersion;

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

            ServerConnection.Put("/api/Applications/Update?name=" + currentApp.name, update);

            newVersion = string.Empty;
            newVersionCurrentPath = "Current path: None";

            developerVM.RefreshList();
        }

        private void Client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            float progress = (float)e.BytesSent / new FileInfo("./tempApp.zip").Length;

            if (progress * 100 < versionPBValue)
                return;

            versionPBValue = (float)Math.Round(progress * 100, 2);
        }
        #endregion
    }
}
