using Forester.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Classes = Forester.Models;

namespace Forester
{
    /// <summary>
    /// Logika interakcji dla klasy DeveloperPanel.xaml
    /// </summary>
    public partial class DeveloperPanel : Window
    {
        List<Classes.Application> publishedApplication;
        public DeveloperPanel()
        {
            InitializeComponent();
            GetApplications();
        }

        void GetApplications()
        {
            string appsJson = ServerConnection.Get($"/api/applications/GetPublished/{Data.currentAccount.id}/{Data.currentAccount.password}");
            publishedApplication = JsonConvert.DeserializeObject<Classes.Application[]>(appsJson).ToList();

            foreach (Classes.Application app in publishedApplication)
            {
                TextBlock nameOfApp = new TextBlock();
                nameOfApp.Text = app.name;

                applist.Items.Add(nameOfApp);
            }
        }

        void RefreshPage(Classes.Application? app)
        {
            if(app is null)
            {
                NameInput.IsEnabled = true;
                NameInput.Text = "";
                DescriptionInput.Text = "";
                AllowedIdsInput.Text = "";
                AllowedIdsInput.IsEnabled = false;
                VersionInput.Text = "";
                isprivate.IsChecked = false;
                InfoUpload.Content = "Wypuść nową aplikacje";
                VersionUpload.Content = "Wypuść wersje (Dostepne po wypuszczeniu)";
                return;
            }

            NameInput.IsEnabled = false;

            NameInput.Text = app.Value.name;
            DescriptionInput.Text = app.Value.description;

            string allowedIdsContent = string.Empty;

            foreach (Account a in app.Value.allowedAccounts)
                allowedIdsContent += a.id + "\n";

            AllowedIdsInput.Text = allowedIdsContent;
            AllowedIdsInput.IsEnabled = app.Value.isPrivate == "true";
            VersionInput.Text = app.Value.version;
            InfoUpload.Content = "Zaaktulizuj dane";
            isprivate.IsChecked = app.Value.isPrivate == "true";
            VersionUpload.Content = "Wypuść wersje";
        }

        void applist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock text = applist.SelectedItem as TextBlock;

            if (text is null)
                return;

            var app = publishedApplication.Single(x => x.name == text.Text);
            RefreshPage(app);
        }

        //Create new button clicked
        void Button_Click(object sender, RoutedEventArgs e) => RefreshPage(null);

        private void InfoUpload_Click(object sender, RoutedEventArgs e)
        {
            List<Account> allowedAccountsList = new List<Account>();

            if(AllowedIdsInput.IsEnabled)
                foreach (string id in AllowedIdsInput.Text.Split(new[] { '\r', '\n' }))
                    if(id != string.Empty)
                        allowedAccountsList.Add(new Account { id = Convert.ToInt32(id) });

            Classes.Application app = new Classes.Application()
            {
                name = NameInput.Text,
                description = DescriptionInput.Text,
                isPrivate = isprivate.IsChecked.ToString().ToLower(),
                version = VersionInput.Text,
                allowedAccounts = allowedAccountsList.ToArray(),
                author = Data.currentAccount,
                isInDownloadFolder = "false"
            };

            VerifiedApplication va = new VerifiedApplication()
            {
                application = app,
                verificationKey = new VerificationKey()
                {
                    id = Data.currentAccount.id,
                    password = Data.currentAccount.password
                }
            };

            if ((string)InfoUpload.Content == "Wypuść nową aplikacje")
                Error.Content = ServerConnection.Post("/api/Applications/PublishNew/", va);
            else
            {
                app.version = VersionInput.Text;
                Error.Content = ServerConnection.Post("/api/Applications/UpdateInfo/", va);
            }

            applist.Items.Clear();
            GetApplications();
            RefreshPage(app);
        }

        #region ToolBox
        void Exit_Clicked(object sender, RoutedEventArgs e) 
        {
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }
        void DragBar_MouseDown(object sender, MouseButtonEventArgs e) => DragMove();
        #endregion

        private void isprivate_Checked(object sender, RoutedEventArgs e) => AllowedIdsInput.IsEnabled = true;

        private void isprivate_Unchecked(object sender, RoutedEventArgs e) => AllowedIdsInput.IsEnabled = false;

        private async void VersionUpload_Click(object sender, RoutedEventArgs e)
        {
            Error.Content = "";
            List<Account> allowedAccountsList = new List<Account>();

            if (AllowedIdsInput.IsEnabled)
                foreach (string id in AllowedIdsInput.Text.Split(new[] { '\r', '\n' }))
                    if (id != string.Empty)
                        allowedAccountsList.Add(new Account { id = Convert.ToInt32(id) });

            Classes.Application app = new Classes.Application()
            {
                name = NameInput.Text,
                description = DescriptionInput.Text,
                isPrivate = isprivate.IsChecked.ToString().ToLower(),
                version = VersionInput.Text,
                allowedAccounts = allowedAccountsList.ToArray(),
                author = Data.currentAccount,
                isInDownloadFolder = "false"
            };

            await DownloadHandler.Upload(app, DirectoryInput.Text);
            Error.Content = "Przesłane";

            applist.Items.Clear();
            GetApplications();
            RefreshPage(app);
        }
    }
}
