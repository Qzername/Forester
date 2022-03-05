using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Forester.Views.AppPages.DeveloperPages
{
    public partial class ManageAppView : UserControl
    {
        public ManageAppView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
