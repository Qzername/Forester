using Forester.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.Popup.SettingsPages
{
    internal class UserProfileViewModel : ViewModelBase, IPage
    {
        /*
         * nie dokończyłem tego bo za bardzo zmęczyłem się robieniem tego projektu
         * patrz DownloadPanelViewModel czy jakoś tak
         */

        public void PageClosed()
        {
        }

        public void PageOpened()
        {
        }

        public ViewModelBase ReceiveContent()
        {
            return this;
        }
    }
}
