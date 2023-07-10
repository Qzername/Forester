using Avalonia.Media;
using Forester.Data;
using Forester.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Globalization;

namespace Forester.Services
{
    public class ThemeService : ReactiveObject
    {
        [Reactive] public string Name { get; private set; }
        [Reactive] public SolidColorBrush FirstBrush { get; private set; }
        [Reactive] public SolidColorBrush SecondBrush { get; private set; }
        [Reactive] public SolidColorBrush ThirdBrush { get; private set; }
        [Reactive] public SolidColorBrush Orange { get; private set; }

        SettingsFile settingsFile;

        public ThemeService(SettingsFile settingsFile) 
        {
            this.settingsFile = settingsFile;

            ReadTheme();
        }

        public void SetTheme(string name)
        {
            settingsFile.SetTheme(name);
            ReadTheme();
        }

        public void SetTheme(Theme theme)
        {
            settingsFile.SetTheme(theme);
            ReadTheme();
        }

        void ReadTheme()
        {
            var theme = settingsFile.Settings.Theme;

            Name = theme.Name;
            FirstBrush = new SolidColorBrush(HexToColor(theme.FirstColor));
            SecondBrush = new SolidColorBrush(HexToColor(theme.SecondColor));
            ThirdBrush = new SolidColorBrush(HexToColor(theme.ThirdColor));

            Orange = new SolidColorBrush(HexToColor("#FFBB00"));
        }

        Color HexToColor(string hexString)
        {
            if (hexString.IndexOf('#') != -1)
                hexString = hexString.Replace("#", "");

            byte r, g, b;

            r = byte.Parse(hexString.Substring(0, 2), NumberStyles.AllowHexSpecifier);
            g = byte.Parse(hexString.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            b = byte.Parse(hexString.Substring(4, 2), NumberStyles.AllowHexSpecifier);

            return Color.FromArgb(255, r, g, b);
        }
    }
}
