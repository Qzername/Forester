using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Forester.Views.App.Store
{
    public partial class DetailedAppView : UserControl
    {
        public DetailedAppView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
