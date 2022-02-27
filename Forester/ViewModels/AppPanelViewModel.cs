using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
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
        SolidColorBrush _first;
        SolidColorBrush _second;
        SolidColorBrush _third;

        public SolidColorBrush first
        {
            get => _first;
            set => this.RaiseAndSetIfChanged(ref _first, value);
        }

        public SolidColorBrush second
        {
            get => _second;
            set => this.RaiseAndSetIfChanged(ref _second, value);
        }

        public SolidColorBrush third
        {
            get => _third;
            set => this.RaiseAndSetIfChanged(ref _third, value);
        }
        #endregion

        SolidColorBrush _storeColor;
        SolidColorBrush _libraryColor;

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

        MainWindowViewModel mainWindowVM;

        public AppPanelViewModel(MainWindowViewModel mainWindowVM)
        {
            username = string.Format("{0}#{1}", Data.account.friendlyUsername, Data.account.friendly_ID);

            profilePicture = ServerConnection.GetImage(username);

            SetTheme(); 

            this.mainWindowVM = mainWindowVM;
            mainWindowVM.toolBarHeight = 20;

            storeColor = first;
            libraryColor = white;
        }

        #region toolbar
        public void Minimalize(Window window) => window.WindowState = WindowState.Minimized;
        public void WindowSize(Window window) => window.WindowState = (window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized);
        public void Exit(Window window)=>window.Close();
        #endregion
        #region theme
        /// <summary>
        /// Ustawienie kolorów
        /// </summary>
        void SetTheme()
        {
            first = new SolidColorBrush(HexToColor(Data.config.theme.colorFirst));
            second = new SolidColorBrush(HexToColor(Data.config.theme.colorSecond));
            third = new SolidColorBrush(HexToColor(Data.config.theme.colorThird));
        }

        /// <summary>
        /// Zamiana hex na kolor (hex może ale nie musi zawierać #)
        /// </summary>
        Color HexToColor(string hexString)
        {
            if (hexString.IndexOf('#') != -1)
                hexString = hexString.Replace("#", "");

            byte r, g, b;

            r = byte.Parse(hexString.Substring(0, 2), NumberStyles.AllowHexSpecifier);
            g = byte.Parse(hexString.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            b = byte.Parse(hexString.Substring(4, 2), NumberStyles.AllowHexSpecifier);

            return Color.FromArgb(255, r, g, b);
        }
        #endregion
    }
}
