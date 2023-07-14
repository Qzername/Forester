using Forester.Data.Connection;
using Forester.Models;
using Forester.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Data
{
    public class VersionDatabase : Database
    {
        protected override string APIprefix => "Update/";

        public VersionDatabase(RequestManager requestManager, FileTransferManager fileTransferManager, ErrorMessageService errorMessageService) : base(requestManager, fileTransferManager, errorMessageService)
        {

        }

        public async Task<string> GetVersion()
        {
            var apiMessage = await RequestManager.Get<ForesterData>(GenerateURI("Version"));

            return ((ForesterData)apiMessage.Content).Version;
        }
        
        public async Task Download(IProgress<int> progress)
        {
            string checksum = File.ReadAllText("./Checksum.json");

            await FileTransferManager.Download(GenerateURI("Download"), "./tempApp.zip", progress, checksum);
        }
    }
}
