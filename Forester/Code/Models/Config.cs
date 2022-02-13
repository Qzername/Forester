using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Models
{
    public struct Config
    {
        public string autoLogin { get; set; }
        public string autoPassword { get; set; }
        public string version { get; set; }
        public Theme theme { get; set; }
    }
}
