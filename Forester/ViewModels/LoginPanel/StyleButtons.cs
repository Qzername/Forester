using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Views
{
    public partial class LoginPanel 
    {
        bool isLogin;

        bool _loginIsEnabled, _registerIsEnabled;
        float _loginOpacity, _registerOpacity;

        public bool loginIsEnabled
        {
            get => _loginIsEnabled;
            set { _loginIsEnabled = value; OnPropertyChanged("loginIsEnabled"); }
        }

        public bool registerIsEnabled
        {
            get => _registerIsEnabled;
            set { _registerIsEnabled = value; OnPropertyChanged("registerIsEnabled"); }
        }

        public float loginOpacity
        {
            get => _loginOpacity;
            set { _loginOpacity = value; OnPropertyChanged("loginOpacity"); }
        }

        public float registerOpacity
        {
            get => _registerOpacity;
            set { _registerOpacity = value; OnPropertyChanged("registerOpacity"); }
        }

        public void SwitchPanels(Button button)
        {
            isLogin = !isLogin;

            loginIsEnabled = isLogin;
            loginOpacity = isLogin ? 1 : 0;

            registerIsEnabled = !isLogin;
            registerOpacity = isLogin ? 0 : 1;

            button.Content = isLogin ? "Don't have an account?" : "Already have an account?";

            login = string.Empty; //From Login.cs
            username = string.Empty; //From Login.cs
            password = string.Empty; //From Login.cs
            error = string.Empty;
        }

        public void Exit(Window window) =>
            window.Close();
    }
}
