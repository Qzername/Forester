using Forester.Models.API;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forester.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        ViewModelBase content;
        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
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

        public MainWindowViewModel()
        {
            width = 1280;
            height = 720;

            Data.ReadConfig();

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
                    Content = new AppPanelViewModel();
                    return;
                }
            }
                
            Content = new LoginPanelViewModel(this);
        }

        public void ChangeWindow<T>() where T : ViewModelBase, new()
        {
            Content = new T();
        }
    }
}
