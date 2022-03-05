using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Forester.Views.AppPages.DeveloperPages
{
    public partial class BasicInfo : UserControl
    {
        public BasicInfo()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
