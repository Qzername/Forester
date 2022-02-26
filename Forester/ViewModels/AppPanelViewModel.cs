using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels
{
    public class AppPanelViewModel : ViewModelBase
    {
        MainWindowViewModel mainWindowVM;

        public AppPanelViewModel(MainWindowViewModel mainWindowVM)
        {
            this.mainWindowVM = mainWindowVM;
            mainWindowVM.toolBarHeight = 20;
        }

    }
}
