using Avalonia.Media;
using Forester.Code.AppData;
using Forester.Models;
using Forester.Models.API;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

/*  name rules
    private variables _[name]
    variables and properties [name]
    struct and classes name [Name] 
    struct and clases objects [name]
    method names [Name]
 */

namespace Forester.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public static MainWindowViewModel Current;

        public SolidColorBrush firstMain;
        public SolidColorBrush secondMain;
        public SolidColorBrush thirdMain;

        ViewModelBase _content;
        public ViewModelBase content
        {
            get => _content;
            private set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        float _width;
        public float width
        {
            get => _width;
            set => this.RaiseAndSetIfChanged(ref _width, value);
        }

        float _height;
        public float height
        {
            get => _height;
            set => this.RaiseAndSetIfChanged(ref _height, value);
        }

        float _toolBarHeight;
        public float toolBarHeight
        {
            get => _toolBarHeight;
            set => this.RaiseAndSetIfChanged(ref _toolBarHeight, value);
        }

        //Pop-up
        PopupConfig _popupConfig;
        public PopupConfig popupConfig
        {
            get => _popupConfig;
            set => this.RaiseAndSetIfChanged(ref _popupConfig, value);
        }

        bool _popupIsEnabled;
        public bool popupIsEnabled
        {
            get => _popupIsEnabled;
            set => this.RaiseAndSetIfChanged(ref _popupIsEnabled, value);
        }

        public float _popupOpacity;
        public float popupOpacity
        {
            get => Convert.ToSingle(popupIsEnabled);
            set => this.RaiseAndSetIfChanged(ref _popupOpacity, value);
        }

        public MainWindowViewModel()
        {
            Current = this;

            width = 1280;
            height = 720;

            popupIsEnabled = false;
            popupConfig = new PopupConfig() { margin = new Avalonia.Thickness(0, 20, 0, 0)};

            Data.ReadConfig();
            SetTheme();

            var version = JsonConverter.Deserialize<Models.API.Version>(ServerConnection.Get("/api/Update/Forester/Version").Content.ReadAsStringAsync().Result);

            if(version.version != Data.config.version)
            {
                Data.config.version = version.version;
                Data.SaveConfig(Data.config);

                var download = new DownloadUpdateViewModel();
                content = download;
                download.Download();
                return;
            }

            //funkcja Remember me 
            if (!string.IsNullOrEmpty(Data.config.autoLogin) && !string.IsNullOrEmpty(Data.config.autoPassword))
            {
                LoginCredentials loginC = new LoginCredentials()
                {
                    username = Data.config.autoLogin,
                    password = Data.config.autoPassword
                };

                var response = ServerConnection.Post("/api/Accounts/Login", loginC);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Data.token = JsonConverter.Deserialize<Token>(response.Content.ReadAsStringAsync().Result);
                    ServerConnection.SetToken(Data.token.token);
                    ChangeToApp(loginC.username);
                    return;
                }
            }
                
            content = new LoginPanelViewModel();
        }

        /// <summary>
        /// Tworzenie pop-upu
        /// </summary>
        public void CreatePopup(PopupConfig popupConfig)
        {
            this.popupConfig = popupConfig;
            ChangeVisibilityPopup(true);
        }

        /// <summary>
        /// zamykanie popupu
        /// </summary>
        public void ClosePopup() => ChangeVisibilityPopup(false);

        /// <summary>
        /// Zmiana widocznoœci okna
        /// </summary>
        public void ChangeVisibilityPopup(bool isVisible) => popupIsEnabled = isVisible;

        /// <summary>
        /// Zmienienie widoku na widok aplikacji
        /// </summary>
        /// <param name="login">nazwa u¿ytkownika</param>
        public void ChangeToApp(string login)
        {
            var response = ServerConnection.Get($"/api/Accounts/GetUser?username={login}");
            Data.account = JsonConverter.Deserialize<Account>(response.Content.ReadAsStringAsync().Result);
            content = new AppPanelViewModel(this);
        }

        /// <summary>
        /// Ustawienie kolorów
        /// </summary>
        void SetTheme()
        {
            firstMain = new SolidColorBrush(HexToColor(Data.config.theme.colorFirst));
            secondMain = new SolidColorBrush(HexToColor(Data.config.theme.colorSecond));
            thirdMain = new SolidColorBrush(HexToColor(Data.config.theme.colorThird));
        }
    }
}
