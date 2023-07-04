using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Models
{
    public struct Theme
    {
        public static Theme[] DefaultThemes = new[]
        {
            new Theme()
            {
                Name = "Spring",
                FirstColor = "9DFF0A",
                SecondColor = "00FF0D",
                ThirdColor = "2fb000"
            },
            new Theme()
            {
                Name = "Summer",
                FirstColor = "FFCB00",
                SecondColor = "F0A900",
                ThirdColor = "FF9B00"
            },
            new Theme()
            {
                Name = "Autumn",
                FirstColor = "FF6508",
                SecondColor = "FF1900",
                ThirdColor = "F00022"
            },
            new Theme()
            {
                Name = "Winter",
                FirstColor = "0DA3FF",
                SecondColor = "0C5BE8",
                ThirdColor = "0019FF"
            },
        };

        public string Name { get; set; }
        public string FirstColor { get; set; }
        public string SecondColor { get; set; }
        public string ThirdColor { get; set; }
    }
}
