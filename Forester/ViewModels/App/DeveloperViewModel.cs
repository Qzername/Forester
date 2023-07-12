using Avalonia.Collections;
using DynamicData;
using Forester.Data;
using Forester.Models.API;
using Forester.Services.App;
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
            GetService<DeveloperService>().RegisterDeveloperPanel(this);

            developedApplications = new AvaloniaList<Application>();
            content = new DeveloperContentViewModel();

            applicationDatabase = GetService<ApplicationDatabase>();

            _ = Refresh();
        }

        public async Task Refresh()
        {
            developedApplications.Clear();
            developedApplications.AddRange(await applicationDatabase.Get());
        }

        public void Move(object nameSTR) => Move((string)nameSTR);

        public void Move(string name)
        {
            content.SwitchTemporaryView(new ManageViewModel(content, developedApplications.Single(x => x.Name == name)));
        }

        public void AddNewClicked()
        {
            content.SwitchTemporaryView(new AddNewViewModel(content));
        }
    }
}
