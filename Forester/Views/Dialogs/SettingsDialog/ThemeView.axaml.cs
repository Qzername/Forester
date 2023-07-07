using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Forester.ViewModels.Dialogs.SettingsDialog;

namespace Forester.Views.Dialogs.SettingsDialog
{
    public partial class ThemeView : ReactiveUserControl<ThemeViewModel>
    {
        public ThemeView()
        {
            InitializeComponent();
        }
    }
}
