using Forester.Models.API;
using Forester.Models.App;
using Forester.ViewModels.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Services.App
{
    public class LibraryService
    {
        LibraryViewModel libraryViewModel;

        public event Action OnApplicationRemoved;

        public void Register(LibraryViewModel libraryViewModel)
        {
            this.libraryViewModel = libraryViewModel;

            libraryViewModel.ApplicationRemoved = LibraryViewModel_OnApplicationRemoved;
        }

        void LibraryViewModel_OnApplicationRemoved()
        {
            OnApplicationRemoved?.Invoke();
        }

        public void AddApplication(Application application) => libraryViewModel.AddAppToList(application);
        public void RemoveApplicaiton(string name) => libraryViewModel.RemoveAppFromList(name);
        public bool IsInLibrary(string name) => libraryViewModel.IsInLibrary(name);
        public void ClearView() => libraryViewModel.ClearView();
        public void AddDownloadSettings(string name, DownloadSettings settings) => libraryViewModel.AddDownloadSettings(name, settings);
    }
}
