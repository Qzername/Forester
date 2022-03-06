using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forester.ViewModels.App.Developer;

namespace Forester.ViewModels.App
{
    public class DeveloperViewModel : ViewModelBase
    {
        ViewModelBase _content;
        ManageAppViewModel manageAppVM;
        CreateAppViewModel createAppVM;

        public ViewModelBase content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        public DeveloperViewModel()
        {
            manageAppVM = new ManageAppViewModel();
            createAppVM = new CreateAppViewModel();

            content = new BasicInfoViewModel();
        }

        public void AddNew() => content = createAppVM;
    }
}
