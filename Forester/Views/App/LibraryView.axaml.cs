using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Forester.ViewModels.App;

namespace Forester.Views.App
{
    public partial class LibraryView : ReactiveUserControl<LibraryViewModel>
    {
        public LibraryView()
        {
            InitializeComponent();
        }
    }
}
