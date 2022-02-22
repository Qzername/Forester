using Avalonia.Controls;
using ForesterAPI.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Views
{
    public partial class LoginPanel
    {
        public string _login;
        public string _username;
        public string _password;
        public string _error;

        public string login
        {
            get => _login;
            set { _login = value; OnPropertyChanged("login"); }
        }

        public string username
        {
            get => _username;
            set { _username = value; OnPropertyChanged("username"); }
        }

        public string password
        {
            get => _password;
            set { _password = value; OnPropertyChanged("password"); }
        }

        public string error
        {
            get => _error;
            set { _error = value; OnPropertyChanged("error"); }
        }

        public void Login()
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                error = "Login and password cannot be empty.";
                return;
            }

            LoginCredentials loginC = new LoginCredentials()
            {
                username = login,
                password = password
            };

            System.Diagnostics.Debug.WriteLine(JsonConverter.Serialize(loginC));

            string response = ServerConnection.Post("/api/Accounts/Login", JsonConverter.Serialize(loginC));

            System.Diagnostics.Debug.WriteLine(response);
        }

        public void Register()
        {

            System.Diagnostics.Debug.WriteLine(password);
        }
    }
}
