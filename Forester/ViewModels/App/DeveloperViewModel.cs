using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forester.ViewModels.App.Developer;
using Forester.Models.API;
using System.Collections.ObjectModel;
using Forester.Code.AppData;

namespace Forester.ViewModels.App
{
    public class DeveloperViewModel : ViewModelBase
    {
        public static DeveloperViewModel Current;

        ViewModelBase _content;
        ManageAppViewModel manageAppVM;
        CreateAppViewModel createAppVM;

        ObservableCollection<Application> apps { get; set; }
        
        public ViewModelBase content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        public bool isSuperDeveloper
        {
            get => Data.account.isDeveloper;
        }

        public DeveloperViewModel()
        {
            apps = new ObservableCollection<Application>();

            RefreshList();

            Current = this;

            manageAppVM = new ManageAppViewModel();
            createAppVM = new CreateAppViewModel();

            content = new BasicInfoViewModel();
        }

        public void RefreshList()
        {
            var response = ServerConnection.Get("/api/Applications/GetDeveloped");

            //musiałem zrobić takie coś zamiast apps.Clear() ponieważ gdy się 
            //użyje tamtej funkcji to te obiekty z niewiadomego powodu gdzieś tam zostają
            for (int i = apps.Count - 1; i > -1; i--)
                apps.RemoveAt(i);

            foreach (var app in JsonConverter.Deserialize<Application[]>(response.Content.ReadAsStringAsync().Result))
                apps.Add(app);
        }

        public void ChangeView(string name)
        {
            Application app = apps.Single(x=>x.name == name);

            content = manageAppVM;
            manageAppVM.SetApp(app);
        }

        public void AddNew() => content = createAppVM;
    }
}
