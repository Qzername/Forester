using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Forester;
using Forester.Models.API;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public string appName, quickDescription, description, currentVersion;

        public string AppName
        {
            get => appName;
            set => this.RaiseAndSetIfChanged(ref appName, value);
        }
        public string QuickDescription
        {
            get => quickDescription;
            set => this.RaiseAndSetIfChanged(ref quickDescription, value);
        }
        public string Description
        {
            get => description;
            set => this.RaiseAndSetIfChanged(ref description, value);
        }
        public string CurrentVersion
        {
            get => currentVersion;
            set => this.RaiseAndSetIfChanged(ref currentVersion, value);
        }

        Bitmap profilePicture;
        public Bitmap ProfilePicture
        {
            get => profilePicture;
            set => this.RaiseAndSetIfChanged(ref profilePicture, value);
        }

        Bitmap backgroundPicture;
        public Bitmap BackgroundPicture
        {
            get => backgroundPicture;
            set => this.RaiseAndSetIfChanged(ref backgroundPicture, value);
        }

        Bitmap tempProfilePicture, tempBackgroundPicture;

        bool makeChanges;
        public bool MakeChanges
        {
            get => makeChanges;
            set => this.RaiseAndSetIfChanged(ref makeChanges, value);
        }

        string switchChangesButtonContent;
        public string SwitchChangesButtonContent
        {
            get => switchChangesButtonContent;
            set => this.RaiseAndSetIfChanged(ref switchChangesButtonContent, value);
        }

        string errorSwitchChanges;
        public string ErrorSwitchChanges
        {
            get => errorSwitchChanges;
            set => this.RaiseAndSetIfChanged(ref errorSwitchChanges, value);
        }

        #endregion

        #region Upload New Version
        string newVersion;
        public string NewVersion
        {
            get => newVersion;
            set => this.RaiseAndSetIfChanged(ref newVersion, value);
        }

        string newVersionCurrentPath;
        public string NewVersionCurrentPath
        {
            get => newVersionCurrentPath;
            set => this.RaiseAndSetIfChanged(ref newVersionCurrentPath, value);
        }

        float versionPBValue;
        public float VersionPBValue
        {
            get => versionPBValue;
            set => this.RaiseAndSetIfChanged(ref versionPBValue, value);
        }

        string errorNewVersion;
        public string ErrorNewVersion
        {
            get => errorNewVersion;
            set => this.RaiseAndSetIfChanged(ref errorNewVersion, value);
        }

        string pathToFile;
        #endregion

        #region Allowance
        #region Private
        ObservableCollection<SingleAllowed> users { get; set; }
        bool isPrivate;
        public bool IsPrivate
        {
            get => isPrivate;
            set
            {
                this.RaiseAndSetIfChanged(ref isPrivate, value);
                IsCheckedHandle();
            }
        }

        string errorPrivate;
        public string ErrorPrivate
        {
            get => errorPrivate;
            set => this.RaiseAndSetIfChanged(ref errorPrivate, value);
        }

        string newUserPrivate;
        public string NewUserPrivate
        {
            get => newUserPrivate;
            set=> this.RaiseAndSetIfChanged(ref newUserPrivate, value);
        }

        Application currentApp;
        
        List<Account> allowedUsers;
        #endregion
        #region Developers
        string errorDeveloper;
        public string ErrorDeveloper
        {
            get => errorDeveloper;
            set => this.RaiseAndSetIfChanged(ref errorDeveloper, value);
        }

        string newDeveloper;
        public string NewDeveloper
        {
            get => newDeveloper;
            set => this.RaiseAndSetIfChanged(ref newDeveloper, value);
        }
        ObservableCollection<SingleAllowed> allowedDevelopers { get; set; }
        List<Account> developers;
        #endregion
        #endregion

        bool _isSuperDeveloper;
        public bool isSuperDeveloper
        {
            get => _isSuperDeveloper;
            set => this.RaiseAndSetIfChanged(ref _isSuperDeveloper, value);
        }

        DeveloperViewModel developerVM;

        public ManageAppViewModel(DeveloperViewModel developerVM)
        {
            users = new ObservableCollection<SingleAllowed>();
            allowedDevelopers = new ObservableCollection<SingleAllowed>();

            allowedUsers = new List<Account>();
            developers = new List<Account>();   

            NewVersionCurrentPath = "Current path: None";
            this.developerVM = developerVM;
        }

        #region Allowance
        #region Private
        public void IsCheckedHandle()
        {
            if (currentApp.isPrivate == IsPrivate.ToString())
                return;

            Application update = new Application()
            {
                ID = 0,
                name = "",
                quickDescription = "",
                description = "",
                isPrivate = IsPrivate.ToString(),
                mainDeveloper = 0,
                downloadNumber = 0,
                version = ""
            };

            ServerConnection.Put("/api/Applications/Update?name=" + currentApp.name, update);
            currentApp.isPrivate = IsPrivate.ToString();
            developerVM.RefreshList();
        }

        public void AddUserPrivate()
        {
            ErrorPrivate = "";

            var username = NewUserPrivate.Split('#');

            if(username.Length < 2)
            {
                ErrorPrivate = "Wrong username";
                return;
            }

            allowedUsers.Add(new Account() 
            {
                ID = 0,
                friendlyUsername = username[0],
                friendly_ID = int.Parse(username[1]),
                description = "",
                isDeveloper = false,
                password = "",
                username = ""
            });

            UpdateDevelopersAllowed();
        }

        public void DeleteUserPrivate()
        {
            ErrorPrivate = "";

            var username = NewUserPrivate.Split('#');

            if(!allowedUsers.Any(x => x.friendlyUsername == username[0] && x.friendly_ID == int.Parse(username[1])))
            {
                ErrorPrivate = "Wrong username";
                return;
            }    

            allowedUsers.Remove(allowedUsers.Single(x=>x.friendlyUsername==username[0] && x.friendly_ID == int.Parse(username[1])));

            UpdateDevelopersAllowed();
        }

        public void SwitchAllowedUser(string name) => NewUserPrivate = name;
        #endregion
        #region Developers
        public void AddDeveloper()
        {
            ErrorDeveloper = "";

            var username = NewDeveloper.Split('#');

            if (username.Length < 2)
            {
                ErrorDeveloper = "Wrong username";
                return;
            }

            developers.Add(new Account()
            {
                ID = 0,
                friendlyUsername = username[0],
                friendly_ID = int.Parse(username[1]),
                description = "",
                isDeveloper = false,
                password = "",
                username = ""
            });

            UpdateDevelopersAllowed(true);
        }

        public void DeleteDeveloper()
        {
            ErrorDeveloper = "";

            var username = NewDeveloper.Split('#');

            if(!developers.Any(x=> x.friendlyUsername == username[0] && x.friendly_ID == int.Parse(username[1])))
            {
                ErrorDeveloper = "Wrong username";
                return;
            }

            developers.Remove(developers.Single(x => x.friendlyUsername == username[0] && x.friendly_ID == int.Parse(username[1])));

            UpdateDevelopersAllowed(true);
        }

        public void SwitchDeveloper(string name) => NewDeveloper = name;
        #endregion
        void UpdateDevelopersAllowed(bool isDeveloper = false)
        {
            Allowed allowed = new Allowed()
            {
                name = currentApp.name,
                allowedDevelopers = developers.ToArray(),
                allowedUsers = allowedUsers.ToArray()
            };

            var response = ServerConnection.Put("/api/Applications/UpdateAllowed", allowed);

            if (response.StatusCode != HttpStatusCode.OK)
                if (isDeveloper)
                    ErrorDeveloper = "User not found";
                else
                    ErrorPrivate = "User not found";
            else
                if(isDeveloper)
                    ErrorDeveloper = "Added";
                else
                    ErrorPrivate = "Added";

            SetAllowedUsers(currentApp);
            
        }

        public void SetAllowedUsers(Application app)
        {
            IsPrivate = bool.Parse(app.isPrivate);

            NewUserPrivate = "";
            ErrorPrivate = "";

            NewDeveloper = "";
            ErrorDeveloper = "";

            for (int i = users.Count - 1; i > -1; i--)
                users.RemoveAt(i); 
            
            for (int i = allowedDevelopers.Count - 1; i > -1; i--)
                allowedDevelopers.RemoveAt(i);

            var response = ServerConnection.Get("/api/Applications/GetAllowed?name=" + app.name);

            var bank = JsonConverter.Deserialize<Allowed>(response.Content.ReadAsStringAsync().Result);

            allowedUsers.Clear();
            if (bank.allowedUsers is not null)
            {
                allowedUsers.AddRange(bank.allowedUsers);

                foreach (Account user in bank.allowedUsers)
                    users.Add(new SingleAllowed() { name = user.friendlyUsername + "#" + user.friendly_ID.ToString() });
            }

            developers.Clear();
            if (bank.allowedDevelopers is not null)
            {
                developers.AddRange(bank.allowedDevelopers);

                foreach (Account user in bank.allowedDevelopers)
                    allowedDevelopers.Add(new SingleAllowed() { name = user.friendlyUsername + "#" + user.friendly_ID.ToString() });
            }
        }
        #endregion

        #region Basic app information 
        public void SwitchChanges()
        {
            if (MakeChanges)
            {
                SwitchChangesButtonContent = "Make changes";
                SetBasicInformation(currentApp);
            }
            else
                SwitchChangesButtonContent = "Cancel changes";

            MakeChanges = !MakeChanges;
        }

        public async void UploadNewProfilePicture()
        {
            var photo = await ReadPhoto();

            if (photo is null)
                return;

            if (tempProfilePicture is null)
                tempProfilePicture = ProfilePicture;

            ProfilePicture = photo;
        }

        public async void UploadNewBackgroundPicture()
        {
            var photo = await ReadPhoto();

           if (photo is null)
                return;

            if(tempBackgroundPicture is null)
                tempBackgroundPicture = BackgroundPicture;

            BackgroundPicture = photo;
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
            ErrorSwitchChanges = "";

            UpdatePhotos();
            UpdateBasicInformation();

            developerVM.RefreshList();

            var response = ServerConnection.Get("/api/Applications/GetSingle?name=" + AppName);
            SetApp(JsonConverter.Deserialize<Application>(response.Content.ReadAsStringAsync().Result));
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

            if (AppName != currentApp.name)
                update.name = AppName;

            if (QuickDescription != currentApp.quickDescription)
                update.quickDescription = QuickDescription;

            if (Description != currentApp.description)
                update.description = Description;

            var response = ServerConnection.Put("/api/Applications/Update?name=" + currentApp.name, update);

            if (!response.IsSuccessStatusCode)
            {
                //kod 499 - nazwa jest już zajęta
                ErrorSwitchChanges = "Name is taken.";
                return;
            }
        }

        void UpdatePhotos()
        {
            //zdjęcia
            if (tempProfilePicture is not null)
                ServerConnection.PostImage(currentApp.name, 1, 0, ProfilePicture);

            if (tempBackgroundPicture is not null)
                ServerConnection.PostImage(currentApp.name, 1, 1, BackgroundPicture);
        }

        void SetBasicInformation(Application app)
        {
            ErrorSwitchChanges = "";

            if (tempProfilePicture is null)
                ProfilePicture = ServerConnection.GetImage(app.name, 1, 0);
            else
                tempProfilePicture = null;

            if(tempBackgroundPicture is null)
                BackgroundPicture = ServerConnection.GetImage(app.name, 1, 1);
            else
                tempBackgroundPicture = null;

            AppName = app.name;
            QuickDescription = app.quickDescription;
            Description = app.description;

            CurrentVersion = "Current version: " + app.version;
        }
        #endregion

        #region Upload New Version


        public async void SelectPath()
        {
            var dialog = new OpenFolderDialog();

            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                string? result = await dialog.ShowAsync(desktop.MainWindow);

                if (result is null)
                    return;

                 pathToFile = result;
                 NewVersionCurrentPath = "Current path: " + pathToFile;
            }
        }

        public void SubmitNewVersion()
        {
            ErrorNewVersion = "";
            VersionPBValue = 0f;

            if (pathToFile == string.Empty)
            {
                ErrorNewVersion = "Path cannot be empty";
                return;
            }

            Thread uploadThread = new Thread(UploadThread);
            uploadThread.Start();
        }

        public async void UploadThread()
        {
            ErrorNewVersion = "Preparing...";

            string path = AppFileManager.PrepareApp(pathToFile);

            /*WebClient client = new WebClient();
            client.UploadProgressChanged += Client_UploadProgressChanged;
            client.UploadFileCompleted += Client_UploadFileCompleted;*/

            ErrorNewVersion = "Uploading...";

            await ServerConnection.Upload(path, "/api/Download/Upload?name=" + currentApp.name);
            Client_UploadFileCompleted(null, null);
        }

        private void Client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            ErrorNewVersion = "Uploading completed.";
            AppFileManager.Clear();

            developerVM.RefreshList();

            CurrentVersion = "Current version: " + NewVersion;

            Application update = new Application()
            {
                ID = 0,
                name = "",
                quickDescription = "",
                description = "",
                isPrivate = "",
                mainDeveloper = 0,
                downloadNumber = 0,
                version = NewVersion
            };

            ServerConnection.Put("/api/Applications/Update?name=" + currentApp.name, update);

            NewVersion = string.Empty;
            NewVersionCurrentPath = "Current path: None";

            developerVM.RefreshList();
        }

        private void Client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            float progress = (float)e.BytesSent / new FileInfo("./tempApp.zip").Length;

            if (progress * 100 < VersionPBValue)
                return;

            VersionPBValue = (float)Math.Round(progress * 100, 2);
        }
        #endregion

        public void SetApp(Application app)
        {
            SwitchChangesButtonContent = "Make changes";
            MakeChanges = false;
            currentApp = app;

            isSuperDeveloper = app.mainDeveloper == Data.account.ID;

            SetAllowedUsers(app);
            SetBasicInformation(app);
        }
    }

    public struct SingleAllowed
    {
        public string name { get; set; }
    }
}
