using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forester.Models.API;
using Forester.Models.App;

namespace Forester.Models
{
    public class Settings //why is this a class and not a struct?
    {
        public string Server { get; set; }
        public Theme Theme { get; set; }
        public Account AutoLogin { get; set; }
        public LibraryElementConfig[] LibraryElements { get; set; }
    }
}
