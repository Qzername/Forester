using Avalonia.Collections;
using Forester.Data;
using Forester.Models.API;
using Forester.Services.App;
using Forester.ViewModels.Bases;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App
{
    public class StoreViewModel : RoutableBase
    {
        AvaloniaList<Application> apps { get; set; }

        //dependency injection
        ApplicationDatabase applicationDatabase;
        LibraryService libraryService;

        public StoreViewModel(IScreen screen) : base(screen)
        { 
            apps = new AvaloniaList<Application>();

            applicationDatabase = GetService<ApplicationDatabase>();
            libraryService = GetService<LibraryService>();

            GetApps();
        }

        public void AddToLibrary(object applicationOBJ)
        {
            libraryService.AddApplication((Application)applicationOBJ);
        }

        async void GetApps()
        {
            apps.Clear();
            apps.AddRange(await applicationDatabase.Get());
        }
    }
}
