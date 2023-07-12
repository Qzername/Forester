using Forester.Data.Connection;
using Forester.Services;
using Forester.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Data
{
    public class ApplicationFileDatabase : Database
    {
        protected override string APIprefix { get; }= "ApplicationFiles/";

        public ApplicationFileDatabase(FileTransferManager fileTransferManager, ErrorMessageService errorMessageService) :base(fileTransferManager, errorMessageService)
        {

        }

        public Task Download()
        {
            throw new NotImplementedException();
        }

        public async Task Upload(string name, string filepath, IProgress<int> progress)
        {
            await FileTransferManager.Upload(GenerateURI($"Upload?name={name}"), File.OpenRead(filepath), progress);
        }
    }
}
