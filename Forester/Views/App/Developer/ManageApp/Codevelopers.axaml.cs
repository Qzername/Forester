using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Forester.Views.App.Developer.ManageApp
{
    public partial class Codevelopers : UserControl
    {
        public Codevelopers()
        {
            InitializeComponent();

            DataContext = new ViewModels.App.Developer.ManageApp.Codevelopers();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
