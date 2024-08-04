using Avalonia.Collections;
using Forester.Models.App;
using Forester.Services.App;
using Forester.Services;
using Forester.ViewModels.Bases;
using System.Threading.Tasks;
using Forester.Data;
using Forester.Models.API.Pictures;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Forester.ViewModels.App.Store
{
    internal class ListViewModel : ViewModelBase
    {
        [Reactive] string searchText { get; set; }

        List<StoreElement> apps;
        AvaloniaList<StoreElement> shownApps { get; set; }

        //dependency injection
        ApplicationDatabase applicationDatabase;
        AccountDatabase accountDatabase;
        LibraryService libraryService;
        PictureService pictureService;
        StoreService storeService;

        public ListViewModel()
        {
            apps = new List<StoreElement>();
            shownApps = new AvaloniaList<StoreElement>();

            applicationDatabase = GetService<ApplicationDatabase>();
            accountDatabase = GetService<AccountDatabase>();
            libraryService = GetService<LibraryService>();
            pictureService = GetService<PictureService>();
            storeService = GetService<StoreService>();

            libraryService.OnApplicationRemoved += LibraryService_OnApplicationRemoved;
        }

        public void MoveTo(object storeElementOBJ)
        {
            storeService.MoveToApp((StoreElement)storeElementOBJ);
        }

        public async Task GetApps()
        {
            var applications = await applicationDatabase.Get();

            StoreElement[] storeElements = new StoreElement[applications.Length];

            for (int i = 0; i < storeElements.Length; i++)
            {
                var currentApp = applications[i];

                storeElements[i] = new StoreElement()
                {
                    Application = currentApp,
                    IsInLibrary = libraryService.IsInLibrary(currentApp.Name),
                    ProfilePicture = await pictureService.GetImage(currentApp.Name, ObjectType.Application, PictureType.ProfilePicture),
                    BackgroundPicture = await pictureService.GetImage(currentApp.Name, ObjectType.Application, PictureType.BackgroundPicture),
                    Owner = await accountDatabase.Get(currentApp.Owner)
                };
            }

            apps.Clear();
            apps.AddRange(storeElements);
            Search();
        }

        public void Search()
        {
            shownApps.Clear();  

            if (string.IsNullOrEmpty(searchText))
            {
                shownApps.AddRange(apps);
                return;
            }

            shownApps.AddRange(apps.Where(x => x.Application.Name.ToLower().Contains(searchText.ToLower())));
        }

        void LibraryService_OnApplicationRemoved()
        {
            for (int i = 0; i < apps.Count; i++)
            {
                var currentApp = apps[i];
                currentApp.IsInLibrary = libraryService.IsInLibrary(currentApp.Application.Name);
                apps[i] = currentApp;
            }
        }
    }
}
