using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD = System.Diagnostics;

namespace Forester.Tools
{
#if DEBUG
    public static class Debug
    {
        public static void Log(string message)
        {
            SD.Debug.WriteLine("---------------------");
            SD.Debug.WriteLine(message);
            SD.Debug.WriteLine("---------------------");
        }

        public static void Log(object message) => Log(message.ToString());
    }
#endif
}
