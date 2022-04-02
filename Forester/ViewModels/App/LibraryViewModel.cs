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
        ObservableCollection<LibraryElement> apps { get; set; }

        ViewModelBase _content;
        public ViewModelBase content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        public StoreViewModel storeVM;
        AppViewModel appVM;

        public LibraryViewModel()
        {
            appVM = new AppViewModel(this);
            content = new DefaultAppViewModel();
            apps = new ObservableCollection<LibraryElement>();
            
            ReadConfig();
        }

        public void AddApp(Application app)
        {
            apps.Add(new LibraryElement()
            {
                appConfig = new AppConfig()
                {
                    id = app.ID,
                    path = "null"
                },
                app = app,
                profilePicture = ServerConnection.GetImage(app.name, 1, 0),
                backgroundPicture = ServerConnection.GetImage(app.name, 1, 1)
             });

            SaveConfig();
        }

        public void SetApp(string appName)
        {
            if(content != appVM)
                content = appVM;

            var app = apps.Single(x => x.app.name == appName);

            appVM.SetApp(app);
        }

        public void DeleteApp(string name)
        {
            var single = apps.Single(x => x.app.name == name);

            apps.Remove(single);

            SaveConfig();
            storeVM.ChangeAllowance(single.app);
        }

        public bool isInLibrary(int idApp) => apps.Any(x=>x.appConfig.id == idApp);   

        void ReadConfig()
        {
            var config = new List<AppConfig>();
            config.AddRange(JsonConverter.Deserialize<AppConfig[]>(FileReader.ReadText("./libraryConfig.json")));

            foreach (AppConfig c in config)
            {
                var response = ServerConnection.Get("/api/Applications/GetSingle?id=" + c.id);
                var app = JsonConverter.Deserialize<Application>(response.Content.ReadAsStringAsync().Result);

                apps.Add(new LibraryElement()
                {
                    appConfig = new AppConfig()
                    {
                        id = c.id,
                        path = c.path
                    },
                    app = app,
                    profilePicture = ServerConnection.GetImage(app.name, 1, 0),
                    backgroundPicture = ServerConnection.GetImage(app.name, 1, 1)
                });
            }    
        }

        void SaveConfig()
        {
            List<AppConfig> configs = new List<AppConfig>();

            foreach (LibraryElement element in apps)
                configs.Add(element.appConfig);

            FileReader.SaveText("./libraryConfig.json", JsonConverter.Serialize(configs.ToArray()));
        }
    }
}
