using Avalonia.Collections;
using Forester.Data;
using Forester.Models.API;
using Forester.Models.API.Pictures;
using Forester.Models.App;
using Forester.Services;
using Forester.Services.App;
using Forester.Tools;
using Forester.ViewModels.App.Library;
using Forester.ViewModels.Bases;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Forester.ViewModels.App
{
    public class LibraryViewModel : RoutableBase
    {
        public Action ApplicationRemoved;

        AvaloniaList<LibraryElement> elements { get; set; }

        //dependency injection
        SettingsFile settingsFile;
        ApplicationDatabase applicationDatabase;
        PictureService pictureService;
        StoreService storeService;

        [Reactive] ViewModelBase content { get; set; }

        public LibraryViewModel(IScreen screen) : base(screen)
        {
            elements = new AvaloniaList<LibraryElement>();
            ClearView();

            GetService<LibraryService>().Register(this);  
            settingsFile = GetService<SettingsFile>();
            applicationDatabase = GetService<ApplicationDatabase>();
            pictureService = GetService<PictureService>();
            storeService = GetService<StoreService>();

            _ = ReadElements();
        }

        async Task ReadElements()
        {
            elements.Clear();

            foreach (var config in settingsFile.Settings.LibraryElements)
            {
                var app = await applicationDatabase.GetById(config.ApplicationID);

                elements.Add(new LibraryElement()
                {
                    Application = app,
                    ProfilePicture = await pictureService.GetImage(app.Name, ObjectType.Application, PictureType.ProfilePicture),
                    BackgroundPicture = await pictureService.GetImage(app.Name, ObjectType.Application, PictureType.BackgroundPicture),
                    DownloadSettings = config.DownloadSettings,
                });
            }

            await storeService.GetApps();
        }

        public void SetApp(object appOBJ) => SetApp((Application)appOBJ);

        public void SetApp(Application application)
        {
            content = new Library.AppViewModel(elements.Single(x=>x.Application.Name == application.Name));
        }

        public async void AddApp(Application application)
        {
            elements.Add(new LibraryElement()
            {
                Application = application,
                ProfilePicture = await pictureService.GetImage(application.Name, ObjectType.Application, PictureType.ProfilePicture),
                BackgroundPicture = await pictureService.GetImage(application.Name, ObjectType.Application, PictureType.BackgroundPicture)
            });

            SaveSettings();
        }

        public void RemoveApp(string name) 
        {
            elements.Remove(elements.Single(x => x.Application.Name == name));

            ApplicationRemoved?.Invoke();

            SaveSettings();
        }

        public void AddDownloadSettings(string name, DownloadSettings settings)
        {
            var element = elements.Single(x => x.Application.Name == name);
            int index = elements.IndexOf(element);

            element.DownloadSettings = settings;
            elements[index] = element;

            SaveSettings();
        }

        public void ClearView()
        {
            content = new DefaultViewModel();
        }

        public bool IsInLibrary(string name) => elements.Any(x => x.Application.Name == name);

        void SaveSettings()
        {
            LibraryElementConfig[] configs = new LibraryElementConfig[elements.Count]; 

            for(int i = 0; i < configs.Length; i++)
            {
                var currentElement = elements[i];

                configs[i] = new LibraryElementConfig()
                {
                    ApplicationID = currentElement.Application.ID,
                    DownloadSettings = currentElement.DownloadSettings,
                };
            }

            settingsFile.SetLibraryElements(configs);
            settingsFile.SaveSettings();
        }
    }
}
