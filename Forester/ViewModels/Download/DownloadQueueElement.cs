using Forester.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.Download
{
    public struct DownloadQueueElement
    {
        public Application Application;
        public DownloadType DownloadType;
        public long Size;
        public Action DownloadFinished;
    }

    public enum DownloadType
    {
        Update, 
        Download,
        Upload
    }
}
