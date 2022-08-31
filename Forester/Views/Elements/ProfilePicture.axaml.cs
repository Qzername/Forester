using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Forester.Views.Elements
{
    public partial class ProfilePicture : UserControl
    {
        public ProfilePicture()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        } 
    }
}
