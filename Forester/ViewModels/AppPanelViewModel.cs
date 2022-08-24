using Avalonia.Controls;
using Avalonia.Media;
using Forester.ViewModels.App;
using ReactiveUI;

namespace Forester.ViewModels
{
    public class AppPanelViewModel : ViewModelBase
    {
        SolidColorBrush storeColor;
        SolidColorBrush libraryColor;
        SolidColorBrush developerColor;

        public SolidColorBrush StoreColor
        {
            get => storeColor;
            set => this.RaiseAndSetIfChanged(ref storeColor, value);
        }

        public SolidColorBrush LibraryColor
        {
            get => libraryColor;
            set => this.RaiseAndSetIfChanged(ref libraryColor, value);
        }

        public SolidColorBrush DeveloperColor
        {
            get => developerColor;
            set => this.RaiseAndSetIfChanged(ref developerColor, value);
        }

        IImage profilePicture;
        public IImage ProfilePicture
        {
            get => profilePicture;
            set => this.RaiseAndSetIfChanged(ref profilePicture, value);
        }

        SolidColorBrush white = new SolidColorBrush(new Color(255, 255, 255, 255));

        string username;
        public string Username
        {
            get => username;
            set => this.RaiseAndSetIfChanged(ref username, value);
        }

        ViewModelBase content;
        public ViewModelBase Content
        {
            get => content;
            set => this.RaiseAndSetIfChanged(ref content, value);
        }

        float _developerOpacity;
        public float developerOpacity
        {
            get => _developerOpacity;
            set => this.RaiseAndSetIfChanged(ref _developerOpacity, value);
        }

        bool developerIsEnabled;
        public bool DeveloperIsEnabled
        {
            get => developerIsEnabled;
            set => this.RaiseAndSetIfChanged(ref developerIsEnabled, value);
        }

        MainWindowViewModel mainWindowVM;

        //Pages
        StoreViewModel storeVM; //ID = 0
        LibraryViewModel libraryVM; //ID = 1
        DeveloperViewModel developerVM; //ID = 2

        public AppPanelViewModel(MainWindowViewModel mainWindowVM)
        {
            libraryVM = new LibraryViewModel();
            storeVM = new StoreViewModel(libraryVM);

            //nie lubię faktu że muszę to zrobić
            libraryVM.storeVM = storeVM;    

            var response = ServerConnection.Get("/api/Applications/GetDeveloped");

            if(Data.account.isDeveloper || JsonConverter.Deserialize<Models.API.Application[]>(response.Content.ReadAsStringAsync().Result).Length > 0)
            {
                developerOpacity = 1;
                DeveloperIsEnabled = true;
                developerVM = new DeveloperViewModel();
            }
            else
            {
                developerOpacity = 0;
                DeveloperIsEnabled = false;
            }

            Content = storeVM;

            Username = string.Format("{0}#{1}", Data.account.friendlyUsername, Data.account.friendly_ID);

            ProfilePicture = ServerConnection.GetImage(Username,0,0);

            this.mainWindowVM = mainWindowVM;
            mainWindowVM.ToolBarHeight = 20;

            StoreColor = First;
            LibraryColor = white;
            DeveloperColor = white;
        }

        public void ChangePage(int pageID)
        {
            switch(pageID)
            {
                case 0:
                    StoreColor = First;
                    LibraryColor = white;
                    DeveloperColor = white;

                    Content = storeVM;
                    break;
                case 1:
                    StoreColor = white;
                    LibraryColor = First;
                    DeveloperColor = white;

                    Content = libraryVM;
                    break;
                case 2:
                    StoreColor = white;
                    LibraryColor = white;
                    DeveloperColor = First;

                    Content = developerVM;
                    break;
            }
        }
    }
}
