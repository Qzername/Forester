using Forester.Data;
using Forester.Data.Connection;
using Forester.Tools;
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
                case TransferAction.Upload: Upload(applicationName, filepath); break;
                case TransferAction.Update: Update(applicationName, filepath); break;
            }
        }

        void Download(string applicationName, string filepath)
        {
            throw new NotImplementedException();
        }

        async Task Upload(string applicationName, string filepath)
        {
            //prepare directory (convert to zip)
            status = "Creating zip file...";

            string zipFile = "./tempApp.zip";
            await Task.Run(() => ZipFile.CreateFromDirectory(filepath, zipFile));

            //upload
            status = "Doing action...";

            await applicationFileDatabase.Upload(applicationName, zipFile, progress);

            ActionCompleted();
        }

        void Update(string applicationName, string filepath)
        {
            throw new NotImplementedException();
        }

        void ActionCompleted()
        {
            status = "Finished";
            isFinished = true;

            if (whenUploadFinished is not null)
                whenUploadFinished.Invoke();
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
