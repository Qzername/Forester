using Forester.Code;
using Forester.Models;
using Forester.Models.API;
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
        ObservableCollection<Application> apps { get; set; }

        List<LibraryElement> elements;

        public StoreViewModel storeVM;

        public LibraryViewModel()
        {
            apps = new ObservableCollection<Application>();
            
            ReadConfig();
        }

        public void AddApp(Application app)
        {
            apps.Add(app);
            elements.Add(new LibraryElement()
            {
                id = app.ID,
                path = "null"
            });

            SaveConfig();
        }

        public void DeleteApp(string name)
        {
            var app = apps.Single(x => x.name == name);

            apps.Remove(app);
            elements.Remove(elements.Single(x => x.id == app.ID));

            SaveConfig();
            storeVM.ChangeAllowance(app);
        }

        public void Download(string name)
        {

        }

        public bool isInLibrary(int idApp) => elements.Any(x=>x.id == idApp);   

        void ReadConfig()
        {
            elements = new List<LibraryElement>();
            elements.AddRange(JsonConverter.Deserialize<LibraryElement[]>(FileReader.ReadText("./libraryConfig.json")));

            foreach (LibraryElement element in elements)
            {
                var response = ServerConnection.Get("/api/Applications/GetSingle?id=" + element.id);
                System.Diagnostics.Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
                apps.Add(JsonConverter.Deserialize<Application>(response.Content.ReadAsStringAsync().Result));
            }    
        }

        void SaveConfig() =>
            FileReader.SaveText("./libraryConfig.json", JsonConverter.Serialize(elements.ToArray()));
    }
}
