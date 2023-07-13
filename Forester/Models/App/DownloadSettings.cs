using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Models.App
{
    public struct DownloadSettings
    {
        public bool IsDownloaded { get; set; }
        public bool UseDefaultPath { get; set; }
        public string Path { get; set; }
        public bool DeleteNotNecessary { get; set; }
    }
}
