using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Forester.Data;
using Forester.Models;
using Forester.Models.Configurations;
using Forester.Services;
using Forester.Tools;
using Forester.ViewModels.Bases;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Forester.ViewModels
{
    public class LoginViewModel : ViewModelBase, IRoutableViewModel
    {
        [Reactive] string login { get; set; }
        [Reactive] string username { get; set; }
        [Reactive] string password { get; set; }
        [Reactive] bool rememberMe { get; set; }
        [Reactive] string error { get; set; }

        //IRoutableViewModel
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

        //dependency injection
        ThemeService theme { get; }
        SettingsFile settingsFile;
        AccountDatabase requestManager;

        public LoginViewModel(IScreen screen)
        {
            HostScreen = screen;

            ResetData();

            theme = GetService<ThemeService>();
            settingsFile = GetService<SettingsFile>();
            requestManager = GetService<AccountDatabase>();

            //window configuration
            var windowConfigurator = GetService<WindowConfigurationService>();
            windowConfigurator.ChangeConfiguration(new WindowConfiguration()
            {
                IsChromeOn = false,
                TitleBarHeight = 50,
            });

            AutoLogin();
        }

        void AutoLogin()
        {
            //if autologin turned off
            if (string.IsNullOrEmpty(settingsFile.Settings.AutoLogin.Login) ||
                string.IsNullOrEmpty(settingsFile.Settings.AutoLogin.Password))
                return;

            login = settingsFile.Settings.AutoLogin.Login;  
            username = settingsFile.Settings.AutoLogin.Username;
            password = settingsFile.Settings.AutoLogin.Password;

            _ = Login();
        }

        public void ResetData()
        {
            login = string.Empty;
            username = string.Empty;
            password = string.Empty;
            rememberMe = false;
            error = string.Empty;
        }

        public async Task Login()
        {
            if (!DoesMeetRequirements(Mode.Login))
                return;

            var account = DataToAccount();

            var apiMessage = await requestManager.Login(account);

            if(apiMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                error = "Login or password is incorrect";
                return;
            }

            if (rememberMe)
                settingsFile.SetAutoLogin(account);

            GetService<UserDataService>().SetAccount(login);

            MoveToAppView();
        }

        public async Task Register()
        {
            if (!DoesMeetRequirements(Mode.Register))
                return;

            var apiMessage = await requestManager.Register(DataToAccount());

            if((int)apiMessage.StatusCode == 499)
            {
                error = "Login already exists in database";
                return;
            }

            _ = Login();
        }

        bool DoesMeetRequirements(Mode mode)
        {
            error = "";

            if (!DoesMeetLengthRequirement(login))
            {
                error = "Login should be 8 to 20 characters long";
                return false;
            }

            if (mode == Mode.Register && !DoesMeetLengthRequirement(username, 4))
            {
                error = "Username should be 4 to 20 characters long";
                return false;
            }

            if (!DoesMeetLengthRequirement(password))
            {
                error = "Password should be 8 to 20 characters long";
                return false;
            }

            return true;
        }

        void MoveToAppView() => HostScreen.Router.Navigate.Execute(new AppViewModel(HostScreen));

        bool DoesMeetLengthRequirement(string text, int min = 8, int max = 20) => (text.Length >= min && text.Length <= max);

        Account DataToAccount() => 
            new Account()
            {
                Login = login,
                Username = username,
                Password = password,
            };

        public void Exit()
        {
            var desktop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
            desktop.Shutdown();
        }

        enum Mode
        {
            Register,
            Login
        }
    }
}
