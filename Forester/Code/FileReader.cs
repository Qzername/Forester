using System.IO;

namespace Forester
{ 
    public static class FileReader
    {
        public static string ReadText(string file) => File.ReadAllText(file);

        public static void SaveText(string file, string text) => File.WriteAllText(file, text);
    }
}
