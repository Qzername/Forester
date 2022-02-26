using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Forester.Views
{
    public partial class AppPanelView : UserControl
    {
        public AppPanelView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
