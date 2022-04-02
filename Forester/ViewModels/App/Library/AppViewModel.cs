using Avalonia.Media.Imaging;
using Forester.Models;
using ReactiveUI;

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

        LibraryElement currentApp;
        LibraryViewModel libraryVM;

        public AppViewModel(LibraryViewModel libraryVM)
        {
            this.libraryVM = libraryVM;
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

        public void Download()
        {

        }

        public void Delete()
        {

        }

        public void DeleteFromLibrary()
        {
            libraryVM.DeleteApp(currentApp.app.name);
            libraryVM.content = new DefaultAppViewModel();
        }
    }
}