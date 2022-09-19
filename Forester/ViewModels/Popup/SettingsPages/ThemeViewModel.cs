using Forester.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.Popup.SettingsPages
{
    internal class ThemeViewModel : ViewModelBase, IPage
    {
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
