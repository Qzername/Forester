using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels
{
    public class LoginPanelViewModel : ReactiveObject
    {
        #region Theme
        SolidColorBrush _first;
        public SolidColorBrush first
        {
            get => _first;
            set => this.RaiseAndSetIfChanged(ref _first, value);
        }
        
        SolidColorBrush _second;
        public SolidColorBrush second
        {
            get => _second;
            set => this.RaiseAndSetIfChanged(ref _second, value);
        }
        
        SolidColorBrush _third;
        public SolidColorBrush third
        {
            get => _third;
            set => this.RaiseAndSetIfChanged(ref _third, value);
        }
        #endregion

        public LoginPanelViewModel()
        {
            Data.ReadConfig();
            SetTheme();
        }

        /// <summary>
        /// Ustawienie kolorów
        /// </summary>
        void SetTheme()
        {
            first = new SolidColorBrush(HexToColor(Data.config.theme.colorFirst));
            second = new SolidColorBrush(HexToColor(Data.config.theme.colorSecond));
            third = new SolidColorBrush(HexToColor(Data.config.theme.colorThird));
        }

        /// <summary>
        /// Zamiana hex na kolor (hex może ale nie musi zawierać #)
        /// </summary>
        Color HexToColor(string hexString)
        {
            if (hexString.IndexOf('#') != -1)
                hexString = hexString.Replace("#", "");

            byte r, g, b = 0;

            r = byte.Parse(hexString.Substring(0, 2), NumberStyles.AllowHexSpecifier);
            g = byte.Parse(hexString.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            b = byte.Parse(hexString.Substring(4, 2), NumberStyles.AllowHexSpecifier);

            return Color.FromArgb(255, r, g, b);
        }
    }
}
