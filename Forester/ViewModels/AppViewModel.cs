using Forester.Services;
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

        public AppViewModel(IScreen screen)
        {
            themeService = GetService<ThemeService>();  

            HostScreen = screen;

            //window configuration
            var windowConfigurationService = GetService<WindowConfigurationService>();
            windowConfigurationService.ChangeConfiguration(new Models.WindowConfiguration()
            {
                IsChromeOn = true,
                TitleBarHeight = 20
            });
        }
    }
}
