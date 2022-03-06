using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Code
{
    public static class AppFileManager
    {
        public static string PrepareApp(string pathToFolder)
        {
            Clear();

            ZipFile.CreateFromDirectory(pathToFolder, "./tempApp.zip");
            return "./tempApp.zip";
        }

        public static void Clear()
        {
            if (File.Exists("./tempApp.zip"))
                File.Delete("./tempApp.zip");
        }
    }
}
