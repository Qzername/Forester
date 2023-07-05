using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using DynamicData;
using Forester.Data;
using Forester.Data.Connection;
using Forester.Models;
using Forester.Services;
using Forester.Tools;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Forester.ViewModels
{
    internal class LoginViewModel : ViewModelBase
    {
        [Reactive] string login { get; set; }
        [Reactive] string username { get; set; }
        [Reactive] string password { get; set; }
        [Reactive] bool rememberMe { get; set; }
        [Reactive] string error { get; set; }

        //dependency injection
        ThemeService theme { get; set; }
        SettingsFile settingsFile;
        AccountDatabase requestManager;

        public LoginViewModel()
        {
            ResetData();

            theme = GetService<ThemeService>();
            settingsFile = GetService<SettingsFile>();
            requestManager = GetService<AccountDatabase>();
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

            var result = await requestManager.Login(DataToAccount());

            Debug.Log(result.StatusCode.ToString());
        }

        public async Task Register()
        {
            if (!DoesMeetRequirements(Mode.Register))
                return;

            var result = await requestManager.Register(DataToAccount());

            Debug.Log(result.StatusCode.ToString());
        }

        bool DoesMeetRequirements(Mode mode)
        {
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

        bool DoesMeetLengthRequirement(string text, int min = 8, int max = 20) => text.Length >= min || text.Length <= max;

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
