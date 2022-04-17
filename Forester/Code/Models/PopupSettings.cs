using Avalonia;
using Forester.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Models
{
    public struct PopupConfig
    {
        public Thickness margin { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public ViewModelBase content { get; set; }
    }
}
