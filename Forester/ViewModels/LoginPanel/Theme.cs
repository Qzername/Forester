using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Views
{
    public partial class LoginPanel
    {
        SolidColorBrush _first;
        SolidColorBrush _second;
        SolidColorBrush _third;

        public SolidColorBrush first
        {
            get => _first;
            set { _first = value; OnPropertyChanged("first"); }
        }

        public SolidColorBrush second
        {
            get => _second;
            set { _second = value; OnPropertyChanged("second"); }
        }

        public SolidColorBrush third
        {
            get => _third;
            set { _third = value; OnPropertyChanged("third"); }
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

            byte r, g, b;

            r = byte.Parse(hexString.Substring(0, 2), NumberStyles.AllowHexSpecifier);
            g = byte.Parse(hexString.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            b = byte.Parse(hexString.Substring(4, 2), NumberStyles.AllowHexSpecifier);

            return Color.FromArgb(255, r, g, b);
        }
    }
}
