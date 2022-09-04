using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Forester.Views.App.Developer.ManageApp
{
    public partial class PrivateAccess : UserControl
    {
        public PrivateAccess()
        {
            InitializeComponent();

            DataContext = new ViewModels.App.Developer.ManageApp.PrivateAccess();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
