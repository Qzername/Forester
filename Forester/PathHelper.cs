using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester
{
    /// <summary>
    /// Tool for getting full path for file
    /// </summary>
    public static class PathHelper
    {
        static string CurrentDirectory => Directory.GetCurrentDirectory();

        public static string GetPath(string path) 
        {
            if (path.StartsWith("./"))
                path = path.Remove(0, 2);

            return Path.Combine(CurrentDirectory, path); 
        }
    }
}
