using Avalonia.Collections;
using Forester.Models.API;
using Forester.Services.App;
using Forester.ViewModels.App.Developer.Manage;
using Forester.ViewModels.Bases;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App.Developer
{
    public class ManageViewModel : RoutableBase
    {
        AvaloniaList<ViewModelBase> viewConstruction { get; set; }

        public ManageViewModel(IScreen screen, Application application) : base(screen) 
        {
            viewConstruction = new AvaloniaList<ViewModelBase>()
            {
                new BasicInformationViewModel(application),
                new UploadNewVersionViewModel(application),
                new AllowedUsersViewModel(application),
                new CodevelopersViewModel(application)
            };
        }
    }
}
