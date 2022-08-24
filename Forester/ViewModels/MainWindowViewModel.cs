using Avalonia.Media;
using Forester.Models;
using Forester.Models.API;
using ReactiveUI;
using System;

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

        public SolidColorBrush FirstColor;
        public SolidColorBrush SecondColor;
        public SolidColorBrush ThirdColor;

        ViewModelBase _content;
        public ViewModelBase Content
        {
            get => _content;
            private set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        float width;
        public float Width
        {
            get => width;
            set => this.RaiseAndSetIfChanged(ref width, value);
        }

        float height;
        public float Height
        {
            get => height;
            set => this.RaiseAndSetIfChanged(ref height, value);
        }

        float toolBarHeight;
        public float ToolBarHeight
        {
            get => toolBarHeight;
            set => this.RaiseAndSetIfChanged(ref toolBarHeight, value);
        }

        //Pop-up
        PopupConfig popupConfig;
        public PopupConfig PopupConfig
        {
            get => popupConfig;
            set => this.RaiseAndSetIfChanged(ref popupConfig, value);
        }

        bool popupIsEnabled;
        public bool PopupIsEnabled
        {
            get => popupIsEnabled;
            set => this.RaiseAndSetIfChanged(ref popupIsEnabled, value);
        }

        public float popupOpacity;
        public float PopupOpacity
        {
            get => Convert.ToSingle(PopupIsEnabled);
            set => this.RaiseAndSetIfChanged(ref popupOpacity, value);
        }

        public MainWindowViewModel()
        {
            Current = this;

            Width = 1280;
            Height = 720;

            PopupIsEnabled = false;
            PopupConfig = new PopupConfig() { margin = new Avalonia.Thickness(0, 20, 0, 0)};

            Data.ReadConfig();
            SetTheme();

            var version = JsonConverter.Deserialize<Models.API.Version>(ServerConnection.Get("/api/Update/Forester/Version").Content.ReadAsStringAsync().Result);

            if(version.version != Data.config.version)
            {
                Data.config.version = version.version;
                Data.SaveConfig(Data.config);

                var download = new DownloadUpdateViewModel();
                Content = download;
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
                
            Content = new LoginPanelViewModel();
        }

        /// <summary>
        /// Tworzenie pop-upu
        /// </summary>
        public void CreatePopup(PopupConfig popupConfig)
        {
            this.PopupConfig = popupConfig;
            ChangeVisibilityPopup(true);
        }

        /// <summary>
        /// zamykanie popupu
        /// </summary>
        public void ClosePopup() => ChangeVisibilityPopup(false);

        /// <summary>
        /// Zmiana widocznoœci okna
        /// </summary>
        public void ChangeVisibilityPopup(bool isVisible) => PopupIsEnabled = isVisible;

        /// <summary>
        /// Zmienienie widoku na widok aplikacji
        /// </summary>
        /// <param name="login">nazwa u¿ytkownika</param>
        public void ChangeToApp(string login)
        {
            var response = ServerConnection.Get($"/api/Accounts/GetUser?username={login}");
            Data.account = JsonConverter.Deserialize<Account>(response.Content.ReadAsStringAsync().Result);
            Content = new AppPanelViewModel(this);
        }

        /// <summary>
        /// Ustawienie kolorów
        /// </summary>
        void SetTheme()
        {
            FirstColor = new SolidColorBrush(HexToColor(Data.config.theme.colorFirst));
            SecondColor = new SolidColorBrush(HexToColor(Data.config.theme.colorSecond));
            ThirdColor = new SolidColorBrush(HexToColor(Data.config.theme.colorThird));
        }
    }
}
