using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Forester.Code.AppData;
using Forester.Code.Models.Pictures;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Forester.Models.API;

namespace Forester.ViewModels.App.Developer.ManageApp
{
    internal class BasicInformation : ViewModelBase
    {
        public static BasicInformation Current;

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

        public BasicInformation()
        {
            Current = this;
        }

        public void SwitchChanges()
        {
            if (makeChanges)
            {
                switchChangesButtonContent = "Make changes";
                SetBasicInformation(ManageAppViewModel.Current.currentApp);
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

            if (tempBackgroundPicture is null)
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

            DeveloperViewModel.Current.RefreshList();

            var response = ServerConnection.Get("/api/Applications/GetSingle?name=" + appName);
            ManageAppViewModel.Current.SetApp(JsonConverter.Deserialize<Application>(response.Content.ReadAsStringAsync().Result));
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

            if (appName != ManageAppViewModel.Current.currentApp.name)
                update.name = appName;

            if (quickDescription != ManageAppViewModel.Current.currentApp.quickDescription)
                update.quickDescription = quickDescription;

            if (description != ManageAppViewModel.Current.currentApp.description)
                update.description = description;

            var response = ServerConnection.Put("/api/Applications/Update?name=" + ManageAppViewModel.Current.currentApp.name, update);

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
                ServerConnection.PostImage(ManageAppViewModel.Current.currentApp.name, 1, 0, profilePicture);

            if (tempBackgroundPicture is not null)
                ServerConnection.PostImage(ManageAppViewModel.Current.currentApp.name, 1, 1, backgroundPicture);
        }

        public void SetBasicInformation(Application app)
        {
            makeChanges = false;
            switchChangesButtonContent = "Make changes";

            errorSwitchChanges = "";

            if (tempProfilePicture is null)
                profilePicture = ImageData.GetImage(app.name, ObjectType.App, PictureType.ProfilePicture);
            else
                tempProfilePicture = null;

            if (tempBackgroundPicture is null)
                backgroundPicture = ImageData.GetImage(app.name, ObjectType.App, PictureType.BackgroundPicture);
            else
                tempBackgroundPicture = null;

            appName = app.name;
            quickDescription = app.quickDescription;
            description = app.description;

            currentVersion = "Current version: " + app.version;
        }

        public void SetCurrentVersion(string version)
        {
            currentVersion = version;
        }
    }
}
