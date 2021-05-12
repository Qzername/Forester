using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Classes = Forester.Models;

namespace Forester
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            Info.Content = Info.Content + $" - name: {Data.currentAccount.name} - id: {Data.currentAccount.id}" + (Data.currentAccount.isDeveloper== "true"? " DEWELOPER":"");

            DownloadButton.Visibility = Visibility.Hidden;

            GetApplications();
        }

        void GetApplications()
        {
            string result = ServerConnection.Get("/api/Applications/GetPublic");
            Classes.Application[] apps = JsonConvert.DeserializeObject<Classes.Application[]>(result);

            foreach (Classes.Application app in apps)
            {
                TextBlock nameOfApp = new TextBlock();
                nameOfApp.Text = app.name;

                applist.Items.Add(nameOfApp);
                Data.publicAppliactions.Add(app);
            }

            result = ServerConnection.Get($"/api/Applications/GetPrivate/{Data.currentAccount.id}/{Data.currentAccount.password}");
            Classes.Application[] appsPrivate = JsonConvert.DeserializeObject<Classes.Application[]>(result);

            foreach (Classes.Application app in appsPrivate)
            {
                TextBlock nameOfApp = new TextBlock();
                nameOfApp.Text = app.name;

                applist.Items.Add(nameOfApp);
                Data.privateAllowedApplications.Add(app);
            }
        }

        #region ToolBox
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
        #endregion

        private void applist_SelectionChanged(object sender, SelectionChangedEventArgs e) => RefreshPage();
        void RefreshPage()
        {
            DownloadButton.Visibility = Visibility.Visible;

            TextBlock text = applist.SelectedItem as TextBlock;

            var app = Data.getAllApps.Single(x => x.name == text.Text);

            AppName.Content = app.name;
            AppDescription.Content = $"Server Version: {app.version}\nStatus: {(app.isPrivate == "false" ? "Public" : "Private")}" +
                $"\nIs Downloaded? {(DownloadHandler.IsDownloaded(app.name) ? $"Yes - Version: {DownloadHandler.GetInfo(app.name).version}" : "No")}";

            Classes.Application appStatus;
            DownloadButton.IsEnabled = true;

            if (DownloadHandler.IsDownloaded(app.name))
            {
                appStatus = DownloadHandler.GetInfo(app.name);
                if (appStatus.version != app.version)
                    DownloadButton.Content = "Aktulizuj";
                else
                {
                    DownloadButton.IsEnabled = false;
                    DownloadButton.Content = "Wersja aktualna";
                }
            }
            else
                DownloadButton.Content = "Pobierz";
        }
        
        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            TextBlock text = applist.SelectedItem as TextBlock;
            var app = Data.getAllApps.Single(x => x.name == text.Text);
            DownloadButton.IsEnabled = false;
            DownloadButton.Content = "Pobieranie";
            DownloadHandler.Download(app);
            RefreshPage();
        }
    }
}
