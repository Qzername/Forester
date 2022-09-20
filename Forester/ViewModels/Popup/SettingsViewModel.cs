using Forester.Code.AppData;
using Forester.ViewModels.Popup.SettingsPages;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Forester.ViewModels.AppPanelViewModel;

namespace Forester.ViewModels.Popup
{
    internal class SettingsViewModel : PageChanger
    {
        public SettingsViewModel() : base()
        {
            Pages.Add(new Page("Info", new InfoViewModel()));
            //Pages.Add(new Page("User Profile", new UserProfileViewModel()));
            Pages.Add(new Page("Theme", new ThemeViewModel()));

            ChangePage(0);
        }

        public void ClosePopUp()
        {
            MainWindowViewModel.Current.ClosePopup();
        }

        public void LogOut()
        {
            Data.config.autoLogin = "";
            Data.config.autoPassword = "";
            Data.account = new Models.API.Account();
            Data.SaveConfig(Data.config);

            MainWindowViewModel.Current.ClosePopup();
            MainWindowViewModel.Current.ChangeToLogin();
        }
    }
}
