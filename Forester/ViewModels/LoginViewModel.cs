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

        SettingsFile settingsFile;
        public ThemeService theme { get; set; }

        public LoginViewModel()
        {
            ResetData();

            settingsFile = GetService<SettingsFile>();
            theme = GetService<ThemeService>();
        }

        public void ResetData()
        {
            login = string.Empty;
            username = string.Empty;
            password = string.Empty;
            rememberMe = false;
            error = string.Empty;
        }

        public void Login()
        {

        }

        public void Register()
        {

        }
        
        public void Exit()
        {
            var desktop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
            desktop.Shutdown();
        }
    }
}
