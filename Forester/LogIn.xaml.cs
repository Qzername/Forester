using System.Windows;
using Forester.Models;
using Newtonsoft.Json;

namespace Forester
{
    /// <summary>
    /// Logika interakcji dla klasy LogIn.xaml
    /// </summary>
    public partial class LogIn : Window
    {
        public LogIn()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(loginInput.Text) || string.IsNullOrWhiteSpace(passwordInput.Text))
            {
                ErrorText.Content = "Login or password is blank";
                return;
            }

            string response = ServerConnection.Get($"/api/Accounts/GetUser/{loginInput.Text}/{passwordInput.Text}/");
            
            if(response == "NO USER FOUND.")
            {
                ErrorText.Content = "Login or password is incorrect";
                return;
            }

            Data.currentAccount = JsonConvert.DeserializeObject<Account>(response);

            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(loginInput.Text) || string.IsNullOrWhiteSpace(passwordInput.Text))
            {
                ErrorText.Content = "Login or password is blank";
                return;
            }

            Account body = new Account()
            {
                id = 0,
                name = loginInput.Text,
                password = passwordInput.Text,
                isDeveloper = "false"
            };

            string response = ServerConnection.Post("/api/Accounts/NewUser", body);
            
            switch(response)
            {
                case "JSON WITHOUT NEEDED INFORMATION.":
                    ErrorText.Content = "Something went wrong.";
                    break;
                case "NAME TAKEN.":
                    ErrorText.Content = "This login is already taken.";
                    break;
                case "OK.":
                    ErrorText.Content = "Registered.";
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
