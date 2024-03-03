using System;
using System.IO;

namespace Forester
{
    /// <summary>
    /// Tool for getting full path for file
    /// </summary>
    public static class PathHelper
    {
        static string CurrentDirectory => AppDomain.CurrentDomain.BaseDirectory + "\\";

        public static string GetPath(string path) 
        {
            if (path.StartsWith("./"))
                path = path.Remove(0, 2);

            return Path.Combine(CurrentDirectory, path); 
        }
    }
}
