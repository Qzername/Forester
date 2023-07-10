using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Forester.ViewModels.App.Developer;

namespace Forester.Views.App.Developer
{
    public partial class DefaultView : ReactiveUserControl<DefaultViewModel>
    {
        public DefaultView()
        {
            InitializeComponent();
        }
    }
}
