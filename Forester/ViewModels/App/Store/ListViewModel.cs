using Avalonia.Collections;
using Forester.Models.App;
using Forester.Services.App;
using Forester.Services;
using Forester.ViewModels.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forester.Data;
using Forester.Models.API.Pictures;

namespace Forester.ViewModels.App.Store
{
    internal class ListViewModel : ViewModelBase
    {
        AvaloniaList<StoreElement> apps { get; set; }

        //dependency injection
        ApplicationDatabase applicationDatabase;
        AccountDatabase accountDatabase;
        LibraryService libraryService;
        PictureService pictureService;
        StoreService storeService;

        public ListViewModel()
        {
            apps = new AvaloniaList<StoreElement>();

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
