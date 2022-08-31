using Forester.Models;
using Forester.Models.API;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App.Store
{
    public class DetailedAppViewModel : ViewModelBase
    {
        LibraryViewModel LibraryVM;
        StoreViewModel StoreVM;

        StoreElement _currentElement;
        public StoreElement CurrentElement
        {
            get => _currentElement;
            set => this.RaiseAndSetIfChanged(ref _currentElement, value);
        }

        public DetailedAppViewModel(LibraryViewModel LibraryVM, StoreViewModel StoreVM)
        {
            this.LibraryVM = LibraryVM;
            this.StoreVM = StoreVM;
        }

        public void Set(StoreElement app)
        {
            CurrentElement = app;
        }

        public void GoBack()
        {
            StoreVM.ChangeView();
        }

        /// <summary>
        /// Dodawanie do biblioteki aplikacje, od razu zaznaczanie jej "już w bibliotece"
        /// </summary>
        /// <param name="name"></param>
        void AddToLibrary()
        {
            LibraryVM.AddApp(CurrentElement.app);

            var copy = CurrentElement;
            copy.isInLibrary = true;
            CurrentElement = copy;
        }
    }
}
