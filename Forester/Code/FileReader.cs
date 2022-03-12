using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.IO;

namespace Forester
{
    public static class FileReader
    {
        public static Bitmap ReadPhoto(string filePath) => new Bitmap(filePath);

        public static string ReadText(string file) => File.ReadAllText(file);

        public static void SaveText(string file, string text) => File.WriteAllText(file, text);
    }
}
