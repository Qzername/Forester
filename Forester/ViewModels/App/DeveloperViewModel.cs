using Avalonia.Collections;
using DynamicData;
using Forester.Data;
using Forester.Models.API;
using Forester.Tools;
using Forester.ViewModels.App.Developer;
using Forester.ViewModels.Bases;
using Forester.Views.App.Developer;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App
{
    public class DeveloperViewModel : RoutableBase
    {
        [Reactive] ContentBase content { get; set; }

        //dependency injection
        ApplicationDatabase applicationDatabase;

        AvaloniaList<Application> developedApplications { get; set; }

        public DeveloperViewModel(IScreen screen) : base(screen)
        {
            content = new DeveloperContentViewModel();

            applicationDatabase = GetService<ApplicationDatabase>();

            developedApplications = new AvaloniaList<Application>();

            GetApplications();
        }

        async void GetApplications()
        {
            developedApplications.AddRange(await applicationDatabase.Get());
        }

        public void AddNewClicked()
        {
            content.SwitchTemporaryView(new AddNewViewModel(content));
        }
    }
}
