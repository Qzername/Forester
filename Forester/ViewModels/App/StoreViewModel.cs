using Avalonia.Collections;
using DynamicData;
using Forester.Data;
using Forester.Models.API;
using Forester.Models.API.Pictures;
using Forester.Models.App;
using Forester.Services;
using Forester.Services.App;
using Forester.Tools;
using Forester.ViewModels.App.Store;
using Forester.ViewModels.Bases;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App
{
    public class StoreViewModel : RoutableBase
    {
        [Reactive] ViewModelBase content { get; set; }

        ListViewModel listViewModel;

        public StoreViewModel(IScreen screen) : base(screen)
        {
            GetService<StoreService>().Register(this);

            listViewModel = new ListViewModel();

            content = listViewModel;
        }

        public void GoToDefault()
        {
            content = listViewModel;
        }

        public async Task GetApps()
        {
            await listViewModel.GetApps();
        }

        public void MoveToApp(StoreElement element)
        {
            content = new PageViewModel(element);
        }
    }
}
