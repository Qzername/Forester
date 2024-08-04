using Avalonia.Controls;
using Forester.ViewModels.App.Store;

namespace Forester.Views.App.Store
{
    public partial class ListView : UserControl
    {
        public ListView()
        {
            InitializeComponent();

            SearchTextBox.KeyUp += SearchTextBox_KeyUp;
        }

        //i hope it is correct way to do this...
        private void SearchTextBox_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key != Avalonia.Input.Key.Enter)
                return;

            var dc = (ListViewModel)DataContext;

            dc.Search();
        }
    }
}
