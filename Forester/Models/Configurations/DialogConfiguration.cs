using Avalonia;
using Forester.ViewModels.Bases;
using Forester.ViewModels.Dialogs;
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
        public DialogBase Content { get; set; }
    }
}
