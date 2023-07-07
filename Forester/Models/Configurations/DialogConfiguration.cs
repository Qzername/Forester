using Avalonia;
using Forester.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Models.Configurations
{
    public struct DialogConfiguration
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public ViewModelBase Content { get; set; }
    }
}
