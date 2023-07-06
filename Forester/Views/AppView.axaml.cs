using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Forester.ViewModels;

namespace Forester.Views
{
    public partial class AppView : ReactiveUserControl<AppViewModel>
    {
        public AppView()
        {
            InitializeComponent();
        }
    }
}
