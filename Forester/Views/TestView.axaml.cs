using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Forester.Views
{
    public partial class TestPanelView : UserControl
    {
        public TestPanelView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
