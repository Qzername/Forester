using System;
using System.Net;
using System.Windows;

namespace Forester
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
    }
}
