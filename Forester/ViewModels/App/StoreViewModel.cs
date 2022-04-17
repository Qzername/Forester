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
            apps = new ObservableCollection<StoreElement>();
            this.libraryVM = libraryVM;
            Refresh();
        }

        /// <summary>
        /// Dodawanie do biblioteki aplikacje, od razu zaznaczanie jej "już w bibliotece"
        /// </summary>
        /// <param name="name"></param>
        public void AddToLibrary(string name)
        {
            var app = apps.Single(x => x.app.name == name);
            int index = apps.IndexOf(app);

            libraryVM.AddApp(app.app);

            app.isInLibrary = true;
            apps[index] = app;
        }

        /// <summary>
        /// Zmiana widoczności w sklepie aplikacji "już w bibliotece" na przeciwną do obecnej
        /// </summary>
        public void ChangeAllowance(Application app)
        {
            int index = apps.IndexOf(apps.Single(x => x.app.ID == app.ID));

            var selectedApp = apps[index];
            selectedApp.isInLibrary = !selectedApp.isInLibrary;
            apps[index] = selectedApp;
        }

        /// <summary>
        /// Odświeżanie aplikacji dostępnych w sklepie
        /// </summary>
        public void Refresh()
        {
            var response = ServerConnection.Get("/api/Applications/Get");

            for (int i = apps.Count - 1; i > -1; i--)
                apps.RemoveAt(i);

            foreach (Application app in JsonConverter.Deserialize<Application[]>(response.Content.ReadAsStringAsync().Result))
                apps.Add(new StoreElement()
                {
                    app = app,
                    isInLibrary = libraryVM.isInLibrary(app.ID)
                });
        }
    }
}
