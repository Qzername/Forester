using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Forester.Views.App.Developer.ManageApp
{
    public partial class BasicInformation : UserControl
    {
        public BasicInformation()
        {
            InitializeComponent();

            DataContext = new ViewModels.App.Developer.ManageApp.BasicInformation();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
