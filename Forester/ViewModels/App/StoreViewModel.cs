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
        ObservableCollection<StoreElement> apps;
        ObservableCollection<StoreElement> Apps
        {
            get => apps;
            set => this.RaiseAndSetIfChanged(ref apps, value);
        }

        LibraryViewModel libraryVM;

        public StoreViewModel(LibraryViewModel libraryVM)
        {
            Apps = new ObservableCollection<StoreElement>();
            this.libraryVM = libraryVM;
            Refresh();
        }

        /// <summary>
        /// Dodawanie do biblioteki aplikacje, od razu zaznaczanie jej "już w bibliotece"
        /// </summary>
        /// <param name="name"></param>
        public void AddToLibrary(string name)
        {
            var app = Apps.Single(x => x.app.name == name);
            int index = Apps.IndexOf(app);

            libraryVM.AddApp(app.app);

            app.isInLibrary = true;
            Apps[index] = app;
        }

        /// <summary>
        /// Zmiana widoczności w sklepie aplikacji "już w bibliotece" na przeciwną do obecnej
        /// </summary>
        public void ChangeAllowance(Application app)
        {
            int index = Apps.IndexOf(Apps.Single(x => x.app.ID == app.ID));

            var selectedApp = Apps[index];
            selectedApp.isInLibrary = !selectedApp.isInLibrary;
            Apps[index] = selectedApp;
        }

        /// <summary>
        /// Odświeżanie aplikacji dostępnych w sklepie
        /// </summary>
        public void Refresh()
        {
            var response = ServerConnection.Get("/api/Applications/Get");

            for (int i = Apps.Count - 1; i > -1; i--)
                Apps.RemoveAt(i);

            foreach (Application app in JsonConverter.Deserialize<Application[]>(response.Content.ReadAsStringAsync().Result))
                Apps.Add(new StoreElement()
                {
                    app = app,
                    isInLibrary = libraryVM.isInLibrary(app.ID)
                });
        }
    }
}
