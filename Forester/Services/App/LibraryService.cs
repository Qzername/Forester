using Forester.Models.API;
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

        public void Register(LibraryViewModel libraryViewModel)
        {
            this.libraryViewModel = libraryViewModel;
        }

        public void AddApplication(Application application) => libraryViewModel.AddApp(application);
        public void RemoveApplicaiton(string name) => libraryViewModel.RemoveApp(name);
    }
}
