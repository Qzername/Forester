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

        public async Task Download(string applicationName, string filepath, IProgress<int> progress)
        {
            await FileTransferManager.Download(GenerateURI("Download?name="+applicationName), filepath, progress);
        }

        public async Task Download(string applicationName, string filepath, IProgress<int> progress, string doNotIncludeJson)
        {
            await FileTransferManager.Download(GenerateURI("Download?name="+applicationName), filepath, progress, doNotIncludeJson);    
        }

        public async Task Upload(string name, string filepath, IProgress<int> progress)
        {
            await FileTransferManager.Upload(GenerateURI($"Upload?name={name}"), File.OpenRead(filepath), progress);
        }

        string DictonaryToJson(Dictionary<string, string> dict)
        {
            var entries = dict.Select(d => string.Format("\"{0}\": \"{1}\"", d.Key, d.Value));
            return "{" + string.Join(",", entries) + "}";
        }
    }
}
