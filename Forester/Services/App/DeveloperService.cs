using Forester.Models.API;
using Forester.ViewModels.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Services.App
{
    public class DeveloperService
    {
        DeveloperViewModel developerViewModel;

        public void RegisterDeveloperPanel(DeveloperViewModel developerViewModel) => this.developerViewModel = developerViewModel;

        public async Task RefreshApplicationList() => await developerViewModel.Refresh();
        public void MoveToApp(string name) => developerViewModel.Move(name);
    }
}
