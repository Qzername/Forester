using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Models.App
{
    public struct Page
    {
        public string Name { get; set; }
        public IRoutableViewModel ViewModel { get; set; }
        public SolidColorBrush Color { get; set; }
    }
}
