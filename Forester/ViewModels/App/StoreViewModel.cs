using Avalonia.Collections;
using DynamicData;
using Forester.Data;
using Forester.Models.API;
using Forester.Models.API.Pictures;
using Forester.Models.App;
using Forester.Services;
using Forester.Services.App;
using Forester.Tools;
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
        AvaloniaList<StoreElement> apps { get; set; }

        //dependency injection
        ApplicationDatabase applicationDatabase;
        AccountDatabase accountDatabase;
        LibraryService libraryService;
        PictureService pictureService;

        public StoreViewModel(IScreen screen) : base(screen)
        {
            GetService<StoreService>().Register(this);

            apps = new AvaloniaList<StoreElement>();

            applicationDatabase = GetService<ApplicationDatabase>();
            accountDatabase = GetService<AccountDatabase>();
            libraryService = GetService<LibraryService>();
            pictureService = GetService<PictureService>();

            libraryService.OnApplicationRemoved += LibraryService_OnApplicationRemoved;
        }

        public void AddToLibrary(object applicationOBJ)
        {
            var currentApp = (Application)applicationOBJ;

            libraryService.AddApplication(currentApp);

            var element = apps.Single(x => x.Application.Name == currentApp.Name);
            int index = apps.IndexOf(element);
            element.IsInLibrary = false;
            apps[index] = element;
        }

        public async Task GetApps()
        {
            apps.Clear();

            var applications = await applicationDatabase.Get();

            StoreElement[] storeElements = new StoreElement[applications.Length];

            for(int i = 0; i <storeElements.Length; i++)
            {
                var currentApp = applications[i];

                storeElements[i] = new StoreElement()
                {
                    Application = currentApp,
                    IsInLibrary = !libraryService.IsInLibrary(currentApp.Name),
                    ProfilePicture = await pictureService.GetImage(currentApp.Name, ObjectType.Application, PictureType.ProfilePicture),
                    BackgroundPicture = await pictureService.GetImage(currentApp.Name, ObjectType.Application, PictureType.BackgroundPicture),
                    Owner = await accountDatabase.Get(currentApp.Owner)
                };
            }

            apps.AddRange(storeElements);
        }

        void LibraryService_OnApplicationRemoved()
        {
            for(int i = 0; i < apps.Count; i++)
            {
                var currentApp = apps[i];
                currentApp.IsInLibrary = !libraryService.IsInLibrary(currentApp.Application.Name);
                apps[i] = currentApp;
            }
        }
    }
}
