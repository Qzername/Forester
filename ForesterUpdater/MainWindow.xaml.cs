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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ForesterUpdater.Models;
using Newtonsoft.Json;
using Forester;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;

namespace ForesterUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var info = Directory.GetParent("./").Parent;

            if (info is null ||!File.Exists(info.FullName + "/config.json"))
                Close();

            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(info.FullName + "/config.json"));

            string version = ServerConnection.Get("/api/Info/GetVersion");

            if (config.version == version)
                Close();

            text.Content = "Downloading " + version + " version";

            Download();
        }

        Task Download()
        {
            //Deleting older version of program
            DirectoryInfo di = new DirectoryInfo("./").Parent;

            foreach (FileInfo fileS in di.GetFiles())
                fileS.Delete();

            //Downloading .zip with app
            WebClient wc = new WebClient();
            wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
            wc.DownloadFileAsync(new Uri(ServerConnection.api + $"/api/Info/Download"), "forester.zip");

            return Task.CompletedTask;
        }


        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            var info = Directory.GetParent("./").Parent;
            ZipFile.ExtractToDirectory("./forester.zip", info.FullName, true);


            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = info.FullName + "\\Forester.exe";
            processInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(info.FullName + "\\Forester.exe");
            Process.Start(processInfo);
            
            Task.Delay(1000);
            File.Delete("./forester.zip");

            Close();
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            bar.Value = e.ProgressPercentage;
        }
    }
}
