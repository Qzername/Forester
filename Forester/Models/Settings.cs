using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forester.Models.API;
using Forester.Models.App;

namespace Forester.Models
{
    public class Settings
    {
        public Theme Theme { get; set; }
        public Account AutoLogin { get; set; }
        public LibraryElementConfig[] LibraryElements { get; set; }
    }
}
