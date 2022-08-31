using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Forester.Views.App.Store
{
    public partial class StoreApp : UserControl
    {
        public StoreApp()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
