using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Classes = Forester.Models;

namespace Forester
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Data.BasePage.appName = AppName.Content.ToString();
            Data.BasePage.appDescription = AppDescription.Content.ToString();

            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            Info.Content = Info.Content + $" - name: {Data.currentAccount.name} - id: {Data.currentAccount.id}" + (Data.currentAccount.isDeveloper== "true"? " DEWELOPER":"");

            DownloadButton.Visibility = Visibility.Hidden;
            if (Data.currentAccount.isDeveloper == "false")
                DeveloperButton.Visibility = Visibility.Hidden;

            GetApplications();
        }

        void GetApplications()
        {
            if(Data.getAllApps.Count > 0)
            {
                Data.privateAllowedApplications.Clear();
                Data.publicAppliactions.Clear();
            }

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
        void Maximize_Clicked(object sender, RoutedEventArgs e) =>
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate () {
                if(WindowState == WindowState.Normal)
                {
                    WindowState = WindowState.Maximized;
                    Thickness thick = applist.Margin;
                    thick.Bottom = 25;
                    DownloadButton.Margin = thick;
                    applist.Margin = thick;
                }
                else
                {
                    WindowState = WindowState.Normal;
                    Thickness thick = applist.Margin;
                    thick.Bottom = 10;
                    DownloadButton.Margin = thick;
                    applist.Margin = thick;
                }
                Activate();
            }));
        void Minimalize_Clicked(object sender, RoutedEventArgs e)
            => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate () {
                WindowState = WindowState.Minimized;
                Activate();}));
        void DragBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                var point = e.GetPosition(this);
                Left = point.X-Window.Width/2;
                Top = point.Y;
            }


            DragMove();
        }

        private void Developer_Click(object sender, RoutedEventArgs e)
        {
            DeveloperPanel panel = new DeveloperPanel();
            panel.Show();
            Close();
        }
        #endregion

        private void applist_SelectionChanged(object sender, SelectionChangedEventArgs e) => RefreshPage();
        void RefreshPage()
        {
            DownloadButton.Visibility = Visibility.Visible;

            TextBlock text = applist.SelectedItem as TextBlock;

            if (text is null)
            {
                AppName.Content = Data.BasePage.appName;
                AppDescription.Content = Data.BasePage.appDescription;
                DownloadButton.IsEnabled = false;
                return;
            }

            var app = Data.getAllApps.Single(x => x.name == text.Text);

            AppName.Content = app.name;
            AppDescription.Content = GenerateDescription(app);

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
        
        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            TextBlock text = applist.SelectedItem as TextBlock;
            var app = Data.getAllApps.Single(x => x.name == text.Text);
            DownloadButton.IsEnabled = false;
            DownloadButton.Content = "Pobieranie";
            bool done = await DownloadHandler.Download(app);

            if (done)
                RefreshPage();
            else
            {
                DownloadButton.Content = "Wystąpił błąd. Czy twórca upoblikował pierwszą wersje?";
                DownloadButton.IsEnabled = true;
            }
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            applist.Items.Clear();
            GetApplications();

            DownloadButton.Visibility = Visibility.Hidden;
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            Data.config.autoLogin = "";
            Data.config.autoPassword = "";

            var config = Data.config;

            System.IO.File.WriteAllText("./config.json", JsonConvert.SerializeObject(config));

            Data.currentAccount = new Classes.Account();

            LogIn login = new LogIn();
            login.Show();
            Close();
        }
    
        string GenerateDescription(Classes.Application app)
        {
            if (string.IsNullOrWhiteSpace(app.description))
                return string.Empty;

            string final = app.description;

            final = final.Replace("{version}", app.version);
            final = final.Replace("{author}", app.author.name);
            final = final.Replace("{isPrivate}", app.isPrivate == "true" ? "tak" : "nie");

            return final;
        }
    }
}