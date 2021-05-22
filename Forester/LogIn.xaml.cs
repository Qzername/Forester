using System.Windows;
using Forester.Models;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Forester
{
    public partial class LogIn : Window
    {
        public LogIn()
        {
            InitializeComponent();

            string fileText = System.IO.File.ReadAllText("./config.json");
            Data.config = JsonConvert.DeserializeObject<Config>(fileText);

            if (string.IsNullOrWhiteSpace(Data.config.autoLogin))
                return;

            string response = ServerConnection.Get($"/api/Accounts/GetUser/{Data.config.autoLogin}/{Data.config.autoPassword}/");

            if (response == "NO USER FOUND.")
                return;

            Data.currentAccount = JsonConvert.DeserializeObject<Account>(response);

            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(loginInput.Text) || string.IsNullOrWhiteSpace(passwordInput.Text))
            {
                ErrorText.Content = "Login lub hasło są puste";
                return;
            }

            string response = ServerConnection.Get($"/api/Accounts/GetUser/{loginInput.Text}/{ ServerConnection.Crypt(passwordInput.Text)}/");
            
            if(response == "NO USER FOUND.")
            {
                ErrorText.Content = "Login lub hasło są niepoprawne";
                return;
            }

            Data.currentAccount = JsonConvert.DeserializeObject<Account>(response);

            if ((bool)autologin.IsChecked)
            {
                Data.config.autoLogin = loginInput.Text;
                Data.config.autoPassword = ServerConnection.Crypt(passwordInput.Text);

                Config config = Data.config;
                string text = JsonConvert.SerializeObject(config);

                var stream = System.IO.File.CreateText("./config.json");
                stream.WriteLine(text);
                stream.Close();
            }

            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(loginInput.Text) || string.IsNullOrWhiteSpace(passwordInput.Text))
            {
                ErrorText.Content = "Login lub hasło są puste";
                return;
            }

            Account body = new Account()
            {
                id = 0,
                name = loginInput.Text,
                password = ServerConnection.Crypt(passwordInput.Text),
                isDeveloper = "false"
            };

            string response = ServerConnection.Post("/api/Accounts/NewUser", body);
            
            switch(response)
            {
                case "JSON WITHOUT NEEDED INFORMATION.":
                    ErrorText.Content = "Coś poszło nie tak.";
                    break;
                case "NAME TAKEN.":
                    ErrorText.Content = "Ten login jest już zajęty.";
                    break;
                case "OK.":
                    ErrorText.Content = "Zarejestrowano.";
                    break;
            }
        }

        private void loginInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (loginInput.Text == "Login")
                loginInput.Text = "";
        }
        private void loginInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (loginInput.Text.Length == 0)
                loginInput.Text = "Login";
        }
        private void passwordInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (passwordInput.Text == "Password")
                passwordInput.Text = "";
        }
        private void passwordInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (passwordInput.Text.Length == 0)
                passwordInput.Text = "Password";
        }
    }
}
