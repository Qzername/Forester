using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Forester.Views.App.Store
{
    public partial class AppStoreView : UserControl
    {
        public AppStoreView()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
