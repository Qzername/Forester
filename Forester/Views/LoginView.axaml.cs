using Avalonia.Controls;

namespace Forester.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        protected override void OnInitialized()
        {
            SwitchPanelsButton.Click += SwitchPanels;
        }

        void SwitchPanels(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => SwitchPanels();

        void SwitchPanels()
        {
            LoginPanel.IsVisible = RegisterPanel.IsVisible;
            RegisterPanel.IsVisible = !RegisterPanel.IsVisible;
        }
    }
}
