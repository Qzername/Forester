using Forester.Code;
using Forester.Models;
using Forester.Models.API;
using Forester.ViewModels.App.Library;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App
{
    public class LibraryViewModel : ViewModelBase
    {
        ObservableCollection<LibraryElement> Apps { get; set; }

        ViewModelBase content;
        public ViewModelBase Content
        {
            get => content;
            set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public StoreViewModel storeVM;
        AppViewModel appVM;

        public LibraryViewModel()
        {
            appVM = new AppViewModel(this);
            Content = new DefaultAppViewModel();
            Apps = new ObservableCollection<LibraryElement>();
            
            ReadConfig();
        }

        /// <summary>
        /// Dodawanie aplikacji do biblioteki
        /// </summary>
        public void AddApp(Application app)
        {
            Apps.Add(new LibraryElement()
            {
                appConfig = new AppConfig()
                {
                    id = app.ID,
                    isDefaultPath = false,
                    deleteNotNecessaryFiles = false,
                    path = "null"
                },
                app = app,
                profilePicture = ServerConnection.GetImage(app.name, 1, 0),
                backgroundPicture = ServerConnection.GetImage(app.name, 1, 1)
             });

            SaveConfig();
        }

        /// <summary>
        /// Ustawianie obecnie wyświetlanej aplikacji w bibliotece
        /// </summary>
        public void SetApp(string appName)
        {
            if(Content != appVM)
                Content = appVM;

            var app = Apps.Single(x => x.app.name == appName);
            int index = Apps.IndexOf(app);

            //Odświeżanie informacji o aplikacji
            var response = ServerConnection.Get("/api/Applications/GetSingle?id=" + app.app.ID);
            app.app = JsonConverter.Deserialize<Application>(response.Content.ReadAsStringAsync().Result);

            appVM.SetApp(app);
        }

        /// <summary>
        /// Usuwanie aplikacji z bilioteki
        /// </summary>
        public void DeleteApp(string name)
        {
            var single = Apps.Single(x => x.app.name == name);

            Apps.Remove(single);

            SaveConfig();
            storeVM.ChangeAllowance(single.app);
        }

        /// <summary>
        /// Sprawdzanie czy aplikacja znajduje się obecnie w bibliotece
        /// </summary>
        /// <param name="idApp">ID aplikacji</param>
        public bool isInLibrary(int idApp) => Apps.Any(x=>x.appConfig.id == idApp);   
        
        /// <summary>
        /// Ustawianie configu aplikacji do bazy biblioteki
        /// </summary>
        /// <param name="appConfig"></param>
        public void SetConfig(AppConfig appConfig)
        {
            int index = Apps.IndexOf(Apps.Single(x => x.appConfig.id == appConfig.id));
            var element = Apps[index];

            Apps[index] = new LibraryElement()
            {
                app = element.app,
                appConfig = appConfig,
                backgroundPicture = element.backgroundPicture,
                profilePicture = element.profilePicture
            }; 

            SaveConfig();
        }

        //Czytanie configu z bibloteki
        void ReadConfig()
        {
            var config = new List<AppConfig>();
            config.AddRange(JsonConverter.Deserialize<AppConfig[]>(FileReader.ReadText("./libraryConfig.json")));

            foreach (AppConfig c in config)
            {
                var response = ServerConnection.Get("/api/Applications/GetSingle?id=" + c.id);
                var app = JsonConverter.Deserialize<Application>(response.Content.ReadAsStringAsync().Result);

                Apps.Add(new LibraryElement()
                {
                    appConfig = new AppConfig()
                    {
                        id = c.id,
                        path = c.path,
                        deleteNotNecessaryFiles = c.deleteNotNecessaryFiles,
                        isDefaultPath = c.isDefaultPath
                    },
                    app = app,
                    profilePicture = ServerConnection.GetImage(app.name, 1, 0),
                    backgroundPicture = ServerConnection.GetImage(app.name, 1, 1)
                });
            }    
        }

        //Zapis configu z biblioteki
        void SaveConfig()
        {
            List<AppConfig> configs = new List<AppConfig>();

            foreach (LibraryElement element in Apps)
                configs.Add(element.appConfig);

            FileReader.SaveText("./libraryConfig.json", JsonConverter.Serialize(configs.ToArray()));
        }
    }
}
