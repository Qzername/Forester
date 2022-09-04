using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Forester.Views.App.Developer.ManageApp
{
    public partial class Allowance : UserControl
    {
        public Allowance()
        {
            InitializeComponent();

            DataContext = new ViewModels.App.Developer.ManageApp.Allowance();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
