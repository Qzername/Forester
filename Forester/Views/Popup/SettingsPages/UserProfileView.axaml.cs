using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Forester.Views.Popup.SettingsPages
{
    public partial class UserProfileView : UserControl
    {
        public UserProfileView()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
