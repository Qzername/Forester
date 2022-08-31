using Forester.Models;
using Forester.Models.API;
using Forester.ViewModels.App.Store;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App
{
    public class StoreViewModel : ViewModelBase
    {
        private ViewModelBase _content;
        
        AppStoreViewModel AppStoreVM;
        DetailedAppViewModel DetailedAppVM;

        public ViewModelBase Content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        public StoreViewModel(LibraryViewModel libraryVM)
        {
            AppStoreVM = new AppStoreViewModel(libraryVM, this);
            DetailedAppVM = new DetailedAppViewModel(libraryVM, this);

            ChangeView();
        }

        public void ChangeView(StoreElement? app = null)
        {
            if (app is null)
                Content = AppStoreVM;
            else
            {
                Content = DetailedAppVM;
                DetailedAppVM.Set(app.Value);
            }
        }

        /// <summary>
        /// Odświeżanie aplikacji dostępnych w sklepie
        /// </summary>
        public void Refresh()
        {
            AppStoreVM.Refresh();
        }
    }
}
