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
        BasicInfoViewModel basicInfoVM;

        public ViewModelBase content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        public DeveloperViewModel()
        {
            manageAppVM = new ManageAppViewModel();
            basicInfoVM = new BasicInfoViewModel();

            content = basicInfoVM;
        }
    }
}
