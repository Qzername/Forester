using Avalonia.Collections;
using Forester.Models;
using Forester.Services;
using Forester.Tools;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.Dialogs.SettingsDialog
{
    public class ThemeViewModel : ViewModelBase, IRoutableViewModel
    {
        public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        public IScreen HostScreen { get; }

        ThemeService theme { get; }

        //Theme
        [Reactive] string FirstCustom { get; set; }
        [Reactive] string SecondCustom { get; set; }
        [Reactive] string ThirdCustom { get; set; }

        AvaloniaList<ThemeCheckBox> ThemeCheckBoxes { get; set; }

        public ThemeViewModel(IScreen screen)
        {
            HostScreen = screen;
            ThemeCheckBoxes = new AvaloniaList<ThemeCheckBox>();

            theme = GetService<ThemeService>();

            SetCheckBoxes();
        }

        void SetCheckBoxes()
        {
            List<ThemeCheckBox> list = new List<ThemeCheckBox>();

            foreach (var defaultTheme in Theme.DefaultThemes)
                list.Add(new ThemeCheckBox()
                {
                    IsChecked = theme.Name == defaultTheme.Name,
                    Name = defaultTheme.Name,
                });

            ThemeCheckBoxes.AddRange(list.ToArray());
        }

        public void ChangeTheme(object nameOBJ)
        {
            string name = (string)nameOBJ;

            theme.SetTheme(name);

            CheckboxHandler();
        }

        public void SetCustomTheme()
        {
            int len = FirstCustom.Length + SecondCustom.Length + ThirdCustom.Length;

            if (len != 18)
                return;

            theme.SetTheme(new Theme()
            {
                Name = "CustomTheme",
                FirstColor = FirstCustom,
                SecondColor = SecondCustom,
                ThirdColor = ThirdCustom,
            });

            CheckboxHandler();
        }

        void CheckboxHandler()
        {
            for (int i = 0; i < ThemeCheckBoxes.Count; i++)
            {
                var current = ThemeCheckBoxes[i];
                current.IsChecked = theme.Name == current.Name;
                ThemeCheckBoxes[i] = current;
            }
        }

        struct ThemeCheckBox
        {
            public bool IsChecked { get; set; }
            public string Name { get; set; }
        }
    }
}
