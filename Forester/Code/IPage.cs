using Forester.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Code
{
    public interface IPage
    {
        public void PageOpened();
        public void PageClosed();
        public ViewModelBase ReceiveContent();
    }
}
