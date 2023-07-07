using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Forester.ViewModels.App;

namespace Forester.Views.App
{
    public partial class DeveloperView : ReactiveUserControl<DeveloperViewModel>
    {
        public DeveloperView()
        {
            InitializeComponent();
        }
    }
}
