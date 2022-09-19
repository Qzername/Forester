using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Forester.Views.Popup.SettingsPages
{
    public partial class ThemeView : UserControl
    {
        public ThemeView()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
