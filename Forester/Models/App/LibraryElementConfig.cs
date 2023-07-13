using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Models.App
{
    public struct LibraryElementConfig
    {
        public int ApplicationID { get; set; }
        public DownloadSettings DownloadSettings { get; set; }
    }
}
