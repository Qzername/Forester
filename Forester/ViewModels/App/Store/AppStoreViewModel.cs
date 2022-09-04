using Forester.Code.AppData;
using Forester.Code.Models.Pictures;
using Forester.Models;
using Forester.Models.API;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App.Store
{
    public class AppStoreViewModel : ViewModelBase
    {
        ObservableCollection<StoreElement> _apps;
        ObservableCollection<StoreElement> apps
        {
            get => _apps;
            set => this.RaiseAndSetIfChanged(ref _apps, value);
        }

        public AppStoreViewModel()
        {
            apps = new ObservableCollection<StoreElement>();
            Refresh();
        }

        public void ChangeView(StoreElement app)
        {
            StoreViewModel.Current.ChangeView(app);
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
                    isInLibrary = LibraryViewModel.Current.IsInLibrary(app.ID),
                    profilePicture = ImageData.GetImage(app.name, ObjectType.App, PictureType.ProfilePicture),
                    backgroundPicture = ImageData.GetImage(app.name, ObjectType.App, PictureType.BackgroundPicture)
                });
        }
    }
}
