using Forester.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.Dialogs
{
    public class DialogBase : ViewModelBase
    {
        protected DialogService dialogService;

        public DialogBase() 
        {
            dialogService = GetService<DialogService>();
        }

        public void CloseDialog()
        {
            dialogService.ChangeVisibility(false);
        }
    }
}
