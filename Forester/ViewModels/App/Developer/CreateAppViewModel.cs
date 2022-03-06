using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Forester.Code;
using Forester.Models.API;
using ReactiveUI;
using System.Net;

namespace Forester.ViewModels.App.Developer
{
    public class CreateAppViewModel : ViewModelBase
    {
        string _name;
        string _version;
        string _pathToFolder;
        string _error;
        float _percent;

        public string name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        public string version
        {
            get => _version;
            set => this.RaiseAndSetIfChanged(ref _version, value);
        }
        public string pathToFolder
        {
            get => _pathToFolder;
            set => this.RaiseAndSetIfChanged(ref _pathToFolder, value);
        }
        public string error
        {
            get => _error;
            set => this.RaiseAndSetIfChanged(ref _error, value);
        }
        public float percent
        {
            get => _percent;
            set => this.RaiseAndSetIfChanged(ref _percent, value);
        }
        
        public CreateAppViewModel()
        {
            pathToFolder = "None";
        }

        public void Publish()
        {
            if (string.IsNullOrEmpty(pathToFolder) || string.IsNullOrEmpty(version) || string.IsNullOrEmpty(name))
            {
                error = "All credentials needs to be fullfield";
                return;
            }

            if(pathToFolder == "None")
            {
                error = "Path cannot be empty";
                return;
            }

            Application app = new Application()
            {
                ID = 0,
                name = name,
                version = version,
                isPrivate = "True",
                description = "",
                quickDescription = "",
                downloadNumber = 0,
                mainDeveloper = 1,
            };

            var result = ServerConnection.Post("/api/Applications/New",app);

            if(!result.IsSuccessStatusCode)
            {
                error = "Something went wrong. Probably name is taken";

                System.Diagnostics.Debug.WriteLine(result.Content.ReadAsStringAsync().Result);

                return;
            }

            //Utworzono nową aplikacje, przeysłam pierwszą wersje
            string path = AppFileManager.PrepareApp(pathToFolder);

            WebClient client = new WebClient();
            client.UploadProgressChanged += Client_UploadProgressChanged;
            ServerConnection.Upload(ref client, "/api/Download/Upload?name="+app.name, path);

            AppFileManager.Clear();
        }

        private void Client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            percent = e.ProgressPercentage;
        }

        public async void SelectFolder()
        {
            var dialog = new OpenFolderDialog();
            
            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                string? result = await dialog.ShowAsync(desktop.MainWindow);
                
                if (result is null)
                    return;

                pathToFolder = result;
            }
        }
    }
}
