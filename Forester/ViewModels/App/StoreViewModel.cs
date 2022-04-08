using Forester.Models;
using Forester.Models.API;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App
{
    public class StoreViewModel : ViewModelBase
    {
        ObservableCollection<StoreElement> _apps;
        ObservableCollection<StoreElement> apps
        {
            get => _apps;
            set => this.RaiseAndSetIfChanged(ref _apps, value);
        }

        LibraryViewModel libraryVM;

        public StoreViewModel(LibraryViewModel libraryVM)
        {
            var response = ServerConnection.Get("/api/Applications/Get");

            apps = new ObservableCollection<StoreElement>();

            foreach (Application app in JsonConverter.Deserialize<Application[]>(response.Content.ReadAsStringAsync().Result))
                apps.Add(new StoreElement()
                {
                    app = app,
                    isInLibrary = libraryVM.isInLibrary(app.ID)
                });

            this.libraryVM = libraryVM;
        }

        public void AddToLibrary(string name)
        {
            var app = apps.Single(x => x.app.name == name);
            int index = apps.IndexOf(app);

            libraryVM.AddApp(app.app);

            app.isInLibrary = true;
            apps[index] = app;
        }

        public void ChangeAllowance(Application app)
        {
            int index = apps.IndexOf(apps.Single(x => x.app.ID == app.ID));

            var selectedApp = apps[index];
            selectedApp.isInLibrary = !selectedApp.isInLibrary;
            apps[index] = selectedApp;
        }
    }
}
