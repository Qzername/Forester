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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public async void AddAppToList(Application application)
        {
            elements.Add(new LibraryElement()
            {
                Application = application,
                ProfilePicture = await pictureService.GetImage(application.Name, ObjectType.Application, PictureType.ProfilePicture),
                BackgroundPicture = await pictureService.GetImage(application.Name, ObjectType.Application, PictureType.BackgroundPicture)
            });

            SaveSettings();
        }

        public void RemoveAppFromList(string name) 
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

        public void Delete(object currentElementObject)
        {
            var currentElement = (LibraryElement)currentElementObject;

            try
            {
                string directoryPath = currentElement.DownloadSettings.RealPath;

                string checksum = File.ReadAllText(directoryPath + "ForesterConfig/checksum.json");

                Dictionary<string, string> files = JsonManager.Deserialize<Dictionary<string, string>>(checksum);

                Directory.Delete(directoryPath + "ForesterConfig/", true);

                foreach (var key in files.Keys)
                    File.Delete(directoryPath + key);

                if (Directory.GetFiles(directoryPath).Length == 0)
                    Directory.Delete(directoryPath, true);

            }
            catch(Exception)
            {

            }

            DownloadSettings downloadSettings = new DownloadSettings();

            AddDownloadSettings(currentElement.Application.Name, downloadSettings);
            ClearView();
        }

        public void DeleteFromLibrary(object currentElementObject)
        {
            try
            {
                Delete(currentElementObject);
            }
            catch (Exception)
            {

            }
            var currentElement = (LibraryElement)currentElementObject;

            RemoveAppFromList(currentElement.Application.Name);
            ClearView();
        }

        /// <summary>
        /// Opens directory where the app is downloaded
        /// used in context menu
        /// </summary>
        public void OpenApplicationDirectory(object currentElementObject)
        {
            var currentElement = (LibraryElement)currentElementObject;

            Process.Start("explorer.exe", $"{currentElement.DownloadSettings.RealPath}\\");
        }

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
