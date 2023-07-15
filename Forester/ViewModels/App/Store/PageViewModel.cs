using Forester.Models.App;
using Forester.Services;
using Forester.Services.App;
using Forester.ViewModels.Bases;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App.Store
{
    internal class PageViewModel : ViewModelBase
    {
        //dependency injection
        LibraryService libraryService;
        StoreService storeService;
        ThemeService theme { get; set; }

        [Reactive] StoreElement currentElement { get; set; }

        public PageViewModel(StoreElement storeElement) 
        {
            currentElement = storeElement;

            libraryService = GetService<LibraryService>();
            storeService = GetService<StoreService>();
            theme = GetService<ThemeService>();
        }

        public void GoBack()
        {
            storeService.GoToDefault();
        }
    
        public void AddToLibrary()
        {
            libraryService.AddApplication(currentElement.Application);

            var copy = currentElement;
            copy.IsInLibrary = true;
            currentElement = copy;

            _ = storeService.GetApps();
        }
    }
}
