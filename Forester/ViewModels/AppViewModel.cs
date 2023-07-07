using Avalonia.Collections;
using Avalonia.Media;
using Forester.Models;
using Forester.Models.App;
using Forester.Models.Configurations;
using Forester.Services;
using Forester.ViewModels.App;
using Forester.ViewModels.Dialogs;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels
{
    public class AppViewModel : PageChanger, IRoutableViewModel
    {
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        public IScreen HostScreen { get; }

        //dependecy injection
        DialogService dialogService { get; }

        AppContentViewModel appContent { get; set; }

        public AppViewModel(IScreen screen) : base()
        {
            HostScreen = screen;

            dialogService = GetService<DialogService>();

            appContent = new AppContentViewModel();

            //window configuration
            var windowConfigurationService = GetService<WindowConfigurationService>();
            windowConfigurationService.ChangeConfiguration(new WindowConfiguration()
            {
                IsChromeOn = true,
                TitleBarHeight = 20
            });

            pages.Add(new Page()
            {
                Name = "Store",
                ViewModel = new StoreViewModel(appContent),
            });
            pages.Add(new Page()
            {
                Name = "Library",
                ViewModel = new LibraryViewModel(appContent),
            });
            pages.Add(new Page()
            {
                Name = "Developer",
                ViewModel = new DeveloperViewModel(appContent),
            });

            SwitchPage("Store");
        }

        public void SettingsClicked()
        {
            dialogService.ChangeConfiguration(new DialogConfiguration()
            {
                Width = 800,
                Height = 550,
                Content = new SettingsDialogViewModel()
            });

            dialogService.ChangeVisibility(true);
        }

        public override void SwitchedPage(Page page)
        {
            if (!appContent.DoesExist(page.Name))
                appContent.AddView(page.Name, page.ViewModel);

            appContent.SwitchView(page.Name);
        }
    }
}
