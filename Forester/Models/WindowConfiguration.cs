using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Models
{
    public struct WindowConfiguration
    {
        public bool IsChromeOn { get; set; }
        public int TitleBarHeight { get; set; }

        public WindowConfiguration(bool isChromeOn, int titleBarHeight)
        {
            IsChromeOn = isChromeOn;
            TitleBarHeight = titleBarHeight;
        }
    }
}
