using Forester.Models.App;
using Forester.Models.Configurations;
using Forester.Services;
using Forester.Services.App;
using Forester.Tools;
using Forester.ViewModels.Bases;
using Forester.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
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

        public void Delete()
        {
            string directoryPath = currentElement.DownloadSettings.Path;

            string checksum = File.ReadAllText(directoryPath + "ForesterConfig/checksum.json");

            Dictionary<string, string> files = JsonManager.Deserialize<Dictionary<string, string>>(checksum);

            Directory.Delete(directoryPath + "ForesterConfig/", true);

            foreach (var key in files.Keys)
                File.Delete(directoryPath + key);

            if (Directory.GetFiles(directoryPath).Length == 0)
                Directory.Delete(directoryPath, true);

            DownloadSettings downloadSettings = new DownloadSettings();

            libraryService.AddDownloadSettings(currentElement.Application.Name, downloadSettings);
            libraryService.ClearView();
        }
        
        public void DeleteFromLibrary()
        {
            try
            {
                Delete();
            }
            catch(Exception)
            {

            }
            libraryService.RemoveApplicaiton(currentElement.Application.Name);
            libraryService.ClearView();
        }
    }
}
