using Forester.Models;
using Forester.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App.Store
{
    public class StoreAppViewModel : ViewModelBase
    {
        StoreViewModel StoreViewModel;

        public StoreAppViewModel(StoreViewModel StoreViewModel)
        {
            this.StoreViewModel = StoreViewModel;
        }

        public void ChangeView(StoreElement app)
        {
            StoreViewModel.ChangeView(app);
        }
    }
}
