using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Forester.Views.App
{
    public partial class DownloadView : UserControl
    {
        public DownloadView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

    }
}
