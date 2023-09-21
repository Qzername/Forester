using Forester.Models.App;
using Forester.Models.Configurations;
using Forester.Services;
using Forester.Services.App;
using Forester.Tools;
using Forester.ViewModels.Bases;
using Forester.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;

namespace Forester.ViewModels.App.Library
{
    public class AppViewModel : ViewModelBase
    {
        LibraryElement currentElement { get; set; }

        //dependency injection
        LibraryService libraryService;
        DialogService dialogService;

        public AppViewModel(LibraryElement element)
        {
            currentElement = element;

            libraryService = GetService<LibraryService>();
            dialogService = GetService<DialogService>();
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
                Content = new FileTransferViewModel(FileTransferViewModel.TransferAction.Update, currentElement.Application.Name, currentElement.DownloadSettings.Path, UpdateFinished),
                Width = FileTransferViewModel.PreferredWidth, 
                Height = FileTransferViewModel.PreferredHeight,
            });
            dialogService.ChangeVisibility(true);
        }

        public void Run()
        {
            var files = Directory.GetFiles(currentElement.DownloadSettings.Path);

            var exes = files.Where(x => x.EndsWith(".exe"));

            if (exes.Count() == 0)
                return;

            var name = currentElement.Application.Name.Split(' ')[0];

            foreach (var exe in exes)
                if(exe.Contains(name))
                {
                    string path = Path.GetDirectoryName(exe);
                    var tempExe = @$"""{exe}""";

                    Process.Start(new ProcessStartInfo { FileName = tempExe, WorkingDirectory = path});
                }
        }

        void UpdateFinished()
        {
            //remove files that update of program removed
            if (!currentElement.DownloadSettings.DeleteNotNecessary)
                return;

            string directoryPath = currentElement.DownloadSettings.Path;

            string getChecksum = File.ReadAllText(directoryPath + "ForesterConfig/checksum.json");
        
            Dictionary<string,string> files = JsonManager.Deserialize<Dictionary<string,string>>(getChecksum);

            var filesToDelete = files.Where(x => x.Value == "REMOVE");

            foreach(var file in filesToDelete)
                File.Delete(directoryPath + file.Key);
        }
    }
}
