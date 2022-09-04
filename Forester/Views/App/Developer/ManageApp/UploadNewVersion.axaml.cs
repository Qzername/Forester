using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Forester.Views.App.Developer.ManageApp
{
    public partial class UploadNewVersion : UserControl
    {
        public UploadNewVersion()
        {
            InitializeComponent();

            DataContext = new ViewModels.App.Developer.ManageApp.UploadNewVersion();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
