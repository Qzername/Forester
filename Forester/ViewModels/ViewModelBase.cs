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
        /// Pierwszy kolor motywu
        /// </summary>
        public SolidColorBrush first
        {
            get => MainWindowViewModel.Current.firstMain;
        }

        /// <summary>
        /// Drugi kolor motywu
        /// </summary>
        public SolidColorBrush second
        {
            get => MainWindowViewModel.Current.secondMain;
        }

        /// <summary>
        /// Trzeci kolor motywu
        /// </summary>
        public SolidColorBrush third
        {
            get => MainWindowViewModel.Current.thirdMain;
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
        /// Zmiana maksymalizacji okna na przeciwn¹ do obecnej
        /// </summary>
        public void WindowSize(Window window) => window.WindowState = (window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized);
        
        /// <summary>
        /// Wyjœcie z okna
        /// </summary>
        public void Exit(Window window) => window.Close();
    }
}
