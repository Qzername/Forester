using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace Forester.Views
{
    public partial class LoginPanelView : UserControl
    {
        double currentOffset = 0;

        public LoginPanelView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

    }
}
