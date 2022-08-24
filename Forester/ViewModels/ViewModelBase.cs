using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Forester.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        /// <summary>
        /// Kolor pierwszy ustawionego obecnie motywu
        /// </summary>
        public SolidColorBrush First
        {
            get => MainWindowViewModel.Current.FirstColor;
        }

        /// <summary>
        /// Kolor drugi ustawionego obecnie motywu
        /// </summary>
        public SolidColorBrush Second
        {
            get => MainWindowViewModel.Current.SecondColor;
        }

        /// <summary>
        /// Kolor trzeci ustawionego obecnie motywu
        /// </summary>
        public SolidColorBrush Third
        {
            get => MainWindowViewModel.Current.ThirdColor;
        }

        /// <summary>
        /// Zamiana hex na kolor (hex mo¿e ale nie musi zawieraæ #)
        /// </summary>
        protected Color HexToColor(string hexString)
        {
            if (hexString.IndexOf('#') != -1)
                hexString = hexString.Replace("#", "");

            byte r, g, b;

            r = byte.Parse(hexString.Substring(0, 2), NumberStyles.AllowHexSpecifier);
            g = byte.Parse(hexString.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            b = byte.Parse(hexString.Substring(4, 2), NumberStyles.AllowHexSpecifier);

            return Color.FromArgb(255, r, g, b);
        }

        /// <summary>
        /// Minimalizacja okna
        /// </summary>
        public void Minimalize(Window window) => window.WindowState = WindowState.Minimized;

        /// <summary>
        /// Zmienienie maksymalizacji okna na przeciwn¹
        /// </summary>
        public void WindowSize(Window window) => window.WindowState = (window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized);

        /// <summary>
        /// Wyjœcie z okna
        /// </summary>
        public void Exit(Window window) => window.Close();
    }
}
