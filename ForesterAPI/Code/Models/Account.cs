using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForesterAPI.Models
{
    public struct Account
    {
        public int id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string isDeveloper { get; set; }
    }
}
