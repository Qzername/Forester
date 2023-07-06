using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Forester.ViewModels;
using ReactiveUI;

namespace Forester.Views
{
    public partial class LoginView : ReactiveUserControl<LoginViewModel>
    {
        const string SwitchButtonLoginOn = "Do not have an account?";
        const string SwitchButtonRegisterOn = "Do have an account?";

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

            SwitchPanelsButton.Content = LoginPanel.IsVisible ? SwitchButtonLoginOn : SwitchButtonRegisterOn;
        }
    }
}
