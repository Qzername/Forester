using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Forester.ViewModels.Dialogs.SettingsDialog;

namespace Forester.Views.Dialogs.SettingsDialog
{
    public partial class AboutView : ReactiveUserControl<AboutViewModel>
    {
        public AboutView()
        {
            InitializeComponent();
        }
    }
}
