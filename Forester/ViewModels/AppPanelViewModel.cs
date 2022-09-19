using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Forester.Code;
using Forester.Code.AppData;
using Forester.Code.Models.Pictures;
using Forester.ViewModels.App;
using Forester.ViewModels.Popup;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels
{
    public class AppPanelViewModel : PageChanger
    {
        SolidColorBrush _storeColor;
        SolidColorBrush _libraryColor;
        SolidColorBrush _developerColor;

        public SolidColorBrush storeColor
        {
            get => _storeColor;
            set => this.RaiseAndSetIfChanged(ref _storeColor, value);
        }

        public SolidColorBrush libraryColor
        {
            get => _libraryColor;
            set => this.RaiseAndSetIfChanged(ref _libraryColor, value);
        }

        public SolidColorBrush developerColor
        {
            get => _developerColor;
            set => this.RaiseAndSetIfChanged(ref _developerColor, value);
        }

        IImage _profilePicture;
        public IImage profilePicture
        {
            get => _profilePicture;
            set => this.RaiseAndSetIfChanged(ref _profilePicture, value);
        }

        string _username;
        public string username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        //Pages
        //StoreViewModel ID = 0
        //LibraryViewModel ID = 1
        //DownloadViewModel ID = 2
        //DeveloperViewModel ID = 3

        float _height;
        public float height
        {
            get => _height;
            set => this.RaiseAndSetIfChanged(ref _height, value);
        }

        public AppPanelViewModel() : base()
        {
            //konstruktor libraryVM musi się wywołac jako pierwszy
            //Przez to że store w konstruktorze refreshuje się
            var libraryVM = new LibraryViewModel();
            var storeVM = new StoreViewModel();
            //var downloadVM = new DownloadViewModel();

            Pages.Add(new Page("Store", storeVM));
            Pages.Add(new Page("Library", libraryVM));
            //Pages.Add(new Page("Download", downloadVM));

            var response = ServerConnection.Get("/api/Applications/GetDeveloped");

            if(Data.account.isDeveloper || JsonConverter.Deserialize<Models.API.Application[]>(response.Content.ReadAsStringAsync().Result).Length > 0)
            {
                var developerVM = new DeveloperViewModel();
                Pages.Add(new Page("Developer", developerVM));
            }

            username = string.Format("{0}#{1}", Data.account.friendlyUsername, Data.account.friendly_ID);

            profilePicture = ImageData.GetImage(username, ObjectType.User, PictureType.ProfilePicture);

            MainWindowViewModel.Current.toolBarHeight = 20;

            ChangePage(0);
        }
        
        public void SettingsClicked()
        {
            MainWindowViewModel.Current.CreatePopup(new Models.PopupConfig()
            {
                content = new SettingsViewModel(),
                width = 1000,
                height = 500,
                margin = new Thickness(0, 20, 0, 0)
            }); 
        }

        public struct Page
        {
            public static SolidColorBrush OnColor = MainWindowViewModel.Current.firstMain;
            public static SolidColorBrush OffColor = new SolidColorBrush(new Color(255, 255, 255, 255));

            public SolidColorBrush Color { get; set; }
            public IPage Content { get; }
            public string Name { get; }

            public Page(string Name, IPage Content)
            {
                this.Name = Name;
                this.Color = OffColor;
                this.Content = Content;
            }
        }
    }
}
