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

            var appsRaw = JsonConverter.Deserialize<Application[]>(response.Content.ReadAsStringAsync().Result);

            StoreElement[] Elements = new StoreElement[appsRaw.Length];

            for (int i = 0; i < appsRaw.Length;i++)
            {
                var app = appsRaw[i];

                Elements[i] = new StoreElement()
                {
                    app = app,
                    isInLibrary = LibraryViewModel.Current.IsInLibrary(app.ID),
                    profilePicture = ImageData.GetImage(app.name, ObjectType.App, PictureType.ProfilePicture),
                    backgroundPicture = ImageData.GetImage(app.name, ObjectType.App, PictureType.BackgroundPicture)
                };
            }

            Elements = Sort(Elements);

            foreach (StoreElement single in Elements)
                apps.Add(single);
        }

        /// <summary>
        /// Sortowanie aplikacji według podanych kryteriów:
        /// - Posiadanie profilu - 1 pkt
        /// - Posiadanie tła - 1 pkt
        /// - Posiadanie szybkiego opisu - 1 pkt
        /// - Posiadanie opisu - 1 pkt
        /// - Pobrania - 1 pkt za każde 100 pobrań
        /// </summary>
        public StoreElement[] Sort(StoreElement[] storeElements)
        {
            List<SortingApplication> sortedElementsRaw = new List<SortingApplication>();

            foreach(StoreElement element in storeElements)
            {
                int points = 0;

                points += ImageData.HasImage(element.app.name, ObjectType.App, PictureType.ProfilePicture) ? 1 : 0;
                points += ImageData.HasImage(element.app.name, ObjectType.App, PictureType.BackgroundPicture) ? 1 : 0;
                points += element.app.quickDescription != "No description" ? 1 : 0;
                points += element.app.description != "No description" ? 1 : 0;
                points += element.app.downloadNumber / 100;

                var sorted = new SortingApplication()
                {
                    points = points,
                    app = element
                };

                sortedElementsRaw.Add(sorted);
            }

            var ordered = sortedElementsRaw.OrderBy(x => x.points).Reverse().ToArray();

            for(int i = 0; i<storeElements.Length;i++)
                storeElements[i] = ordered[i].app;

            return storeElements;
        }

        struct SortingApplication
        {
            public int points;
            public StoreElement app;
        }
    }
}
