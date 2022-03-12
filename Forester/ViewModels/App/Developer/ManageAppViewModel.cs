using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Forester.Models.API;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App.Developer
{
    public class ManageAppViewModel : ViewModelBase
    {
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

        Application currentApp;
        DeveloperViewModel developerVM;

        public ManageAppViewModel(DeveloperViewModel developerVM)
        {
            this.developerVM = developerVM;
        }

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

        public void SetApp(Application app)
        {
            switchChangesButtonContent = "Make changes";
            makeChanges = false;
            currentApp = app;

            SetBasicInformation(app);
        }
    }
}
