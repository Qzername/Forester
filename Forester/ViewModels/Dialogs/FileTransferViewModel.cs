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
        [Reactive] string action { get; set; }
        [Reactive] int value { get; set; }

        Progress<int> progress { get; set; }

        //dependency injection
        ApplicationFileDatabase applicationFileDatabase;

        public FileTransferViewModel(Action action,string applicationName, string filepath)
        {
            this.action = action.ToString().Trim('e') + "ing..."; //i hate myself too

            progress = new Progress<int>(percent => { value = percent; });
            applicationFileDatabase = Locator.Current.GetService<ApplicationFileDatabase>()!;

            switch(action)
            {
                case Action.Download: Download(applicationName, filepath); break;
                case Action.Upload: Upload(applicationName, filepath); break;
                case Action.Update: Update(applicationName, filepath); break;
            }
        }

        void Download(string applicationName, string filepath)
        {
            throw new NotImplementedException();
            CloseDialog();
        }

        async void Upload(string applicationName, string filepath)
        {
            ClearZip();

            //prepare directory (convert to zip)
            string zipFile = "./tempApp.zip";
            ZipFile.CreateFromDirectory(filepath, zipFile);

            //upload
            try
            {
                await applicationFileDatabase.Upload(applicationName, zipFile, progress);

                Debug.Log("5");
            }
            catch(Exception ex)
            {
                Debug.Log(ex.Message);
            }

            Debug.Log("6");
            CloseDialog();
        }

        void Update(string applicationName, string filepath)
        {
            throw new NotImplementedException();
            CloseDialog();
        }

        void ClearZip()
        {
            if (File.Exists("./tempApp.zip"))
                File.Delete("./tempApp.zip");
        }

        public enum Action
        {
            Upload,
            Download,
            Update,
        }
    }
}
