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

        public MainWindowViewModel()
        {
            Width = 1280;
            Height = 720;

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
