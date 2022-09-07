using Forester.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels
{
    internal class DownloadViewModel : ViewModelBase, IPage
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
