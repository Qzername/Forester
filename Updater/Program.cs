using System.Diagnostics;
using System.IO.Compression;

string current = Directory.GetCurrentDirectory();

ZipFile.ExtractToDirectory(current + "/tempApp.zip", current, true);

var process = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "./Forester.exe"
    }
};

process.Start();

File.Delete("./tempApp.zip");