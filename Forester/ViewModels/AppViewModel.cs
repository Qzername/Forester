using Forester.Models.Configurations;
using Forester.Services;
using Forester.ViewModels.Dialogs;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels
{
    public class AppViewModel : ViewModelBase, IRoutableViewModel
    {
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        public IScreen HostScreen { get; }

        //dependecy injection
        ThemeService themeService { get; }
        DialogService dialogService { get; }

        public AppViewModel(IScreen screen)
        {
            themeService = GetService<ThemeService>();
            dialogService = GetService<DialogService>();

            HostScreen = screen;

            //window configuration
            var windowConfigurationService = GetService<WindowConfigurationService>();
            windowConfigurationService.ChangeConfiguration(new WindowConfiguration()
            {
                IsChromeOn = true,
                TitleBarHeight = 20
            });
        }

        public void SettingsClicked()
        {
            dialogService.ChangeConfiguration(new DialogConfiguration()
            {
                Width = 500,
                Height = 400,
                Content = new SettingsDialogViewModel()
            });

            dialogService.ChangeVisibility(true);
        }
    }
}
