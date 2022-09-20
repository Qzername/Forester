using Avalonia.FreeDesktop.DBusIme;
using Forester.Code;
using Forester.Code.AppData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Data;
using Avalonia.Styling;

namespace Forester.ViewModels.Popup.SettingsPages
{
    internal class ThemeViewModel : ViewModelBase, IPage
    {
        ObservableCollection<Theme> _themes;
        ObservableCollection<Theme> Themes { get => _themes; set => this.RaiseAndSetIfChanged(ref _themes, value); }
        
        bool _isCustom;

        public bool IsCustom
        {
            get => _isCustom;
            set => this.RaiseAndSetIfChanged(ref _isCustom, value);
        }

        string _first, _second, _third;

        public string FirstCustom
        {
            get => _first;
            set => this.RaiseAndSetIfChanged(ref _first, value);
        }
        
        public string SecondCustom
        {
            get => _second;
            set => this.RaiseAndSetIfChanged(ref _second, value);
        }
        
        public string ThirdCustom
        {
            get => _third;
            set => this.RaiseAndSetIfChanged(ref _third, value);
        }

        public ThemeViewModel()
        {
            Themes = new ObservableCollection<Theme>();

            Themes.Add(new Theme()
            {
                Name = "Spring",
                IsChecked = false,
                FirstColor = "9DFF0A",
                SecondColor = "00FF0D",
                ThirdColor = "2fb000"
            });

            Themes.Add(new Theme()
            {
                Name = "Summer",
                IsChecked = false,
                FirstColor = "FFCB00",
                SecondColor = "F0A900",
                ThirdColor = "FF9B00"
            });

            Themes.Add(new Theme()
            {
                Name = "Autumn",
                IsChecked = false,
                FirstColor = "FF6508",
                SecondColor = "FF1900",
                ThirdColor = "F00022"
            });

            Themes.Add(new Theme()
            {
                Name = "Winter",
                IsChecked = false,
                FirstColor = "0DA3FF",
                SecondColor = "0C5BE8",
                ThirdColor = "0019FF"
            });

            if (Themes.Any(x => x.Name == Data.config.theme.Name))
            {
                var current = Themes.Single(x => x.Name == Data.config.theme.Name);
                int i = Themes.IndexOf(current);

                current.IsChecked = true;

                Themes[i] = current;
            }
            else
            {
                IsCustom = true;

                FirstCustom = Data.config.theme.colorFirst;
                SecondCustom = Data.config.theme.colorSecond;
                ThirdCustom = Data.config.theme.colorThird;
            }    
        }

        public void ChangeTheme(string Name)
        {
            for(int i = 0; i < Themes.Count; i++)
            {
                var current = Themes[i];

                current.IsChecked = false;

                Themes[i] = current;
            }

            var selected = Themes.Single(x => x.Name == Name);
            int selectedI = Themes.IndexOf(selected);

            selected.IsChecked = true;

            Themes[selectedI] = selected;

            Data.config.theme = new Models.Theme()
            {
                colorFirst = selected.FirstColor,
                colorSecond = selected.SecondColor,
                colorThird = selected.ThirdColor,
                Name = selected.Name
            };

            Data.SaveConfig(Data.config);
        }

        public void SetCustomTheme()
        {
            int len = FirstCustom.Length + SecondCustom.Length + ThirdCustom.Length;

            if (len != 18)
                return;

            Data.config.theme = new Models.Theme()
            {
                colorFirst = FirstCustom,
                colorSecond = SecondCustom,
                colorThird = ThirdCustom,
                Name = "custom"
            };

            Data.SaveConfig(Data.config);
        }

        public void PageClosed()
        {
        }

        public void PageOpened()
        {
        }

        public ViewModelBase ReceiveContent()
        {
            return this;
        }

        struct Theme
        {
            public bool IsChecked { get; set; }
            public string Name { get; set; }
            public string FirstColor { get; set; }
            public string SecondColor { get; set; }
            public string ThirdColor { get; set; }
        }
    }
}
