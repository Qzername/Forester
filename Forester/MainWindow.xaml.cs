using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace Forester
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFileAsync(
                    new Uri("http://localhost:5000/api/Download"),
                    "./test.zip"
                );
            }

        }

        void Exit_Clicked(object sender, RoutedEventArgs e) => Close();
        void Maximize_Clicked(object sender, RoutedEventArgs e) => Application.Current.MainWindow.WindowState = Application.Current.MainWindow.WindowState==WindowState.Normal? WindowState.Maximized:WindowState.Normal;
        void Minimalize_Clicked(object sender, RoutedEventArgs e) => Application.Current.MainWindow.WindowState = WindowState.Minimized;
        void DragBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
                var point = e.GetPosition(this);
                Left = point.X-Window.Width/2;
                Top = point.Y;
            }    

            DragMove();
        }
    }
}
