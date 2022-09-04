using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Forester.Code.AppData;
using Forester.Code.Models.Pictures;
using Forester.Models.API;
using Forester.ViewModels.App.Developer.ManageApp;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Forester.ViewModels.App.Developer
{
    public class ManageAppViewModel : ViewModelBase
    {
        public static ManageAppViewModel Current;

        public Application currentApp;

        bool _isSuperDeveloper;
        public bool isSuperDeveloper
        {
            get => _isSuperDeveloper;
            set => this.RaiseAndSetIfChanged(ref _isSuperDeveloper, value);
        }

        public ManageAppViewModel()
        {
            Current = this;
        }

        public void SetApp(Application app)
        {
            currentApp = app;

            isSuperDeveloper = app.mainDeveloper == Data.account.ID;

            BasicInformation.Current.SetBasicInformation(app);
            Allowance.Current.SetAllowedUsers(app);
        }
    }

    public struct SingleAllowed
    {
        public string name { get; set; }
    }
}
