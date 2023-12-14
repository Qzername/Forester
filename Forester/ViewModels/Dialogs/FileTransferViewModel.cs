using Forester.Data;
using Forester.Data.Connection;
using Forester.Tools;
using Microsoft.VisualBasic.FileIO;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.Dialogs
{
    public class FileTransferViewModel : DialogBase
    {
        public const int PreferredWidth = 500;
        public const int PreferredHeight = 140;

        [Reactive] string action { get; set; }
        [Reactive] string status { get; set; }
        [Reactive] int value { get; set; }
        [Reactive] bool isFinished { get; set; }

        Progress<int> progress { get; set; }
        Action? whenUploadFinished;

        //dependency injection
        ApplicationFileDatabase applicationFileDatabase;

        public FileTransferViewModel(TransferAction action,string applicationName, string filepath, Action? whenUploadFinished = null)
        {
            isFinished = false;
            this.action = action.ToString().Trim('e') + "ing"; //i hate myself too

            progress = new Progress<int>(percent => { value = percent; });
            this.whenUploadFinished = whenUploadFinished;

            applicationFileDatabase = Locator.Current.GetService<ApplicationFileDatabase>()!;

            ClearZip();

            switch (action)
            {
                case TransferAction.Download: Download(applicationName, filepath); break;
                case TransferAction.Update: Update(applicationName, filepath); break;
                case TransferAction.Upload: Upload(applicationName, filepath); break;
            }
        }

        async Task Download(string applicationName, string filepath) => await Download(applicationName, filepath, string.Empty);

        async Task Update(string applicationName, string filepath)
        {
            if (!filepath.EndsWith("\\") && !filepath.EndsWith("/"))
                filepath += "\\";

            string checksum = File.ReadAllText(filepath + "ForesterConfig/checksum.json");

            await Download(applicationName, filepath, checksum);
        }

        async Task Download(string applicationName, string filepath, string doNotInclude)
        {
            //download
            status = "Doing action...";

            if (doNotInclude == string.Empty)
                await applicationFileDatabase.Download(applicationName, "./tempApp.zip", progress);
            else
                await applicationFileDatabase.Download(applicationName, "./tempApp.zip", progress, doNotInclude);

            //unziping
            status = "Unzipping...";
            Task zipFileUnpacking = Task.Run(() => ZipFile.ExtractToDirectory("./tempApp.zip", filepath, true));
            await zipFileUnpacking;

            ActionCompleted();
        }

        async Task Upload(string applicationName, string filepath)
        {
            //prepare directory (convert to zip)
            status = "Creating zip file...";

            string zipFile = "./tempApp.zip";

            Task zipFileCreation = Task.Run(() => ZipFile.CreateFromDirectory(filepath, zipFile));
            await zipFileCreation;

            //upload
            status = "Doing action...";

            await applicationFileDatabase.Upload(applicationName, zipFile, progress);

            ActionCompleted();
        }

        void ActionCompleted()
        {
            status = "Finished";
            isFinished = true;

            whenUploadFinished?.Invoke();
        }

        void ClearZip()
        {
            if (File.Exists("./tempApp.zip"))
                File.Delete("./tempApp.zip");
        }

        public enum TransferAction
        {
            Upload,
            Download,
            Update,
        }
    }
}
