using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Forester.ViewModels.App;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels
{
    public class AppPanelViewModel : ViewModelBase
    {
        #region theme
        public SolidColorBrush first
        {
            get => MainWindowViewModel.current.first;
        }

        public SolidColorBrush second
        {
            get => MainWindowViewModel.current.second;
        }

        public SolidColorBrush third
        {
            get => MainWindowViewModel.current.third;
        }
        #endregion

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

        SolidColorBrush white = new SolidColorBrush(new Color(255, 255, 255, 255));

        string _username;
        public string username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        ViewModelBase _content;
        public ViewModelBase content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        float _developerOpacity;
        public float developerOpacity
        {
            get => _developerOpacity;
            set => this.RaiseAndSetIfChanged(ref _developerOpacity, value);
        }

        bool _developerIsEnabled;
        public bool developerIsEnabled
        {
            get => _developerIsEnabled;
            set => this.RaiseAndSetIfChanged(ref _developerIsEnabled, value);
        }

        MainWindowViewModel mainWindowVM;

        //Pages
        StoreViewModel storeVM; //ID = 0
        LibraryViewModel libraryVM; //ID = 1
        DeveloperViewModel developerVM; //ID = 2

        float _height;
        public float height
        {
            get => _height;
            set => this.RaiseAndSetIfChanged(ref _height, value);
        }

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
                developerIsEnabled = true;
                developerVM = new DeveloperViewModel();
            }
            else
            {
                developerOpacity = 0;
                developerIsEnabled = false;
            }

            content = storeVM;

            username = string.Format("{0}#{1}", Data.account.friendlyUsername, Data.account.friendly_ID);

            profilePicture = ServerConnection.GetImage(username,0,0);

            this.mainWindowVM = mainWindowVM;
            mainWindowVM.toolBarHeight = 20;

            storeColor = first;
            libraryColor = white;
            developerColor = white;
        }

        public void ChangePage(int pageID)
        {
            switch(pageID)
            {
                case 0:
                    storeColor = first;
                    libraryColor = white;
                    developerColor = white;

                    content = storeVM;
                    break;
                case 1:
                    storeColor = white;
                    libraryColor = first;
                    developerColor = white;

                    content = libraryVM;
                    break;
                case 2:
                    storeColor = white;
                    libraryColor = white;
                    developerColor = first;

                    content = developerVM;
                    break;
            }
        }

        #region toolbar
        public void Minimalize(Window window) => window.WindowState = WindowState.Minimized;
        public void WindowSize(Window window) => window.WindowState = (window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized);
        public void Exit(Window window)=>window.Close();
        #endregion
    }
}
