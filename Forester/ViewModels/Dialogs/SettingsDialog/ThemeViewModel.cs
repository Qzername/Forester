using Avalonia.Collections;
using Forester.Data;
using Forester.Models;
using Forester.Services;
using Forester.Tools;
using Forester.ViewModels.Bases;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Forester.ViewModels.Dialogs.SettingsDialog
{
    public class ThemeViewModel : RoutableBase
    {
        ThemeService theme { get; }

        //Theme
        [Reactive] string FirstCustom { get; set; }
        [Reactive] string SecondCustom { get; set; }
        [Reactive] string ThirdCustom { get; set; }

        [Reactive] bool IsCustom { get; set; }
        [Reactive] string hexError { get; set; }

        AvaloniaList<ThemeCheckBox> ThemeCheckBoxes { get; set; }

        public ThemeViewModel(IScreen screen) : base(screen) 
        {
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

            if(theme.Name == "Custom")
            {
                var settingFile = GetService<SettingsFile>();

                FirstCustom = settingFile.Settings.Theme.FirstColor;
                SecondCustom = settingFile.Settings.Theme.SecondColor;
                ThirdCustom = settingFile.Settings.Theme.ThirdColor;

                IsCustom = true;
            }

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
            hexError = "";

            if (FirstCustom is null || SecondCustom is null || ThirdCustom is null)
            {
                hexError = "All text blocks has to be fullfield.";
                return;
            }

            int len = FirstCustom.Length + SecondCustom.Length + ThirdCustom.Length;

            if (len != 18)
            {
                hexError = "Some colors arent valid hex colors.";
                return;
            }

            if (!CheckIfItsHex(FirstCustom) || !CheckIfItsHex(SecondCustom) || !CheckIfItsHex(ThirdCustom))
            {
                hexError = "Some colors arent valid hex colors.";
                return;
            }

            theme.SetTheme(new Theme()
            {
                Name = "CustomTheme",
                FirstColor = FirstCustom,
                SecondColor = SecondCustom,
                ThirdColor = ThirdCustom,
            });

            CheckboxHandler();
        }

        bool CheckIfItsHex(string hc) => Regex.IsMatch(hc, @"[0-9A-Fa-f]{6}\b");

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
