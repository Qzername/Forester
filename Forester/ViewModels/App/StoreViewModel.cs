using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App
{
    public class StoreViewModel : ViewModelBase, IRoutableViewModel
    {
        //IRoutableViewModel
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

        public StoreViewModel(IScreen screen) 
        {
            HostScreen = screen;
        }
    }
}
