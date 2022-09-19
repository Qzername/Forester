using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Forester.Views.Download
{
    public partial class CurrentDownloadedApp : UserControl
    {
        public CurrentDownloadedApp()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
