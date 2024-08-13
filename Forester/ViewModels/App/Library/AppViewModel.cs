using Forester.Data;
using Forester.Models;
using Forester.Models.App;
using Forester.Models.Configurations;
using Forester.Services;
using Forester.Services.App;
using Forester.Tools;
using Forester.ViewModels.Bases;
using Forester.ViewModels.Dialogs;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Forester.ViewModels.App.Library
{
    public class AppViewModel : ViewModelBase
    {
        [Reactive] bool showRunButton { get; set; }
        [Reactive] bool updateAccessible { get; set; }

        LibraryElement currentElement { get; set; }

        //dependency injection
        DialogService dialogService;

        public AppViewModel(LibraryElement element)
        {
            currentElement = element;

            dialogService = GetService<DialogService>();

            VerifyVisibilityOfRunButton();
            DetectUpdate();
        }

        public void Download()
        {
            dialogService.ChangeConfiguration(new DialogConfiguration()
            {
                Content = new DownloadSettingsViewModel(currentElement.Application.Name),
                Width = 600,
                Height = 300,
            });
            dialogService.ChangeVisibility(true);
        }

        public void Update()
        {
            dialogService.ChangeConfiguration(new DialogConfiguration()
            {
                Content = new FileTransferViewModel(FileTransferViewModel.TransferAction.Update, currentElement.Application.Name, currentElement.DownloadSettings.RealPath, UpdateFinished),
                Width = FileTransferViewModel.PreferredWidth, 
                Height = FileTransferViewModel.PreferredHeight,
            });
            dialogService.ChangeVisibility(true);
        }

        public void Run()
        {
            if (!VerifyVisibilityOfRunButton())
                return;

            var files = Directory.GetFiles(currentElement.DownloadSettings.RealPath);

            var name = currentElement.Application.Name.Split(' ')[0];

            foreach (var exe in files.Where(x => x.EndsWith(".exe")))
                if(exe.Contains(name))
                {
                    string path = Path.GetDirectoryName(exe);
                    var tempExe = @$"""{exe}""";

                    Process.Start(new ProcessStartInfo { FileName = tempExe, WorkingDirectory = path});
                }
        }

        void UpdateFinished()
        {
            VerifyVisibilityOfRunButton();
            DetectUpdate();

            //older versions of ForesterAPI will not create version.json, so here we check that it is correct manually
            if(updateAccessible)
            {
                File.Delete(currentElement.DownloadSettings.RealPath + "/ForesterConfig/version.json");
                DetectUpdate();
            }

            //remove files that update of program removed
            if (!currentElement.DownloadSettings.DeleteNotNecessary)
                return;

            string directoryPath = currentElement.DownloadSettings.RealPath;

            string getChecksum = File.ReadAllText(directoryPath + "ForesterConfig/checksum.json");

            Dictionary<string, string> files = JsonManager.Deserialize<Dictionary<string, string>>(getChecksum);

            var filesToDelete = files.Where(x => x.Value == "REMOVE");

            foreach(var file in filesToDelete)
                if(File.Exists(directoryPath + file.Key))
                    File.Delete(directoryPath + file.Key);
        }

        /// <summary>
        /// Checks if the run button should be visible or not
        /// Automaticly changes the value
        /// </summary>
        bool VerifyVisibilityOfRunButton()
        {
            if (!currentElement.DownloadSettings.IsDownloaded)
            {
                showRunButton = false;
                return false;
            }

            var files = Directory.GetFiles(currentElement.DownloadSettings.RealPath);

            showRunButton = files.Any(x => x.EndsWith(".exe"));

            return showRunButton;
        }

        async void DetectUpdate()
        {
            updateAccessible = false;

            var appSerivce = GetService<ApplicationDatabase>();

            string versionFile = currentElement.DownloadSettings.RealPath + "/ForesterConfig/version.json";

            if (!File.Exists(versionFile))
            {
                File.WriteAllText(versionFile
                    , "{ \"Version\":\"" + (await appSerivce.GetByName(currentElement.Application.Name)).Version + "\"}");
            }

            var json = File.ReadAllText(versionFile);
            var version = JsonManager.Deserialize<VersionData>(json);

            updateAccessible = version.Version != currentElement.Application.Version;
        }
    }
}
