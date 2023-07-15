using Forester.Models.App;
using Forester.ViewModels.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Services.App
{
    public class StoreService
    {
        StoreViewModel storeViewModel;

        public void Register(StoreViewModel storeViewModel)
        {
            this.storeViewModel = storeViewModel;
        }

        public async Task GetApps() => await storeViewModel.GetApps();

        //ui
        public void MoveToApp(StoreElement element) => storeViewModel.MoveToApp(element);
        public void GoToDefault() => storeViewModel.GoToDefault();
    }
}
