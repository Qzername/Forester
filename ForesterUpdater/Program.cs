using System.Diagnostics;
using System.IO.Compression;

//forester will run this app in his directory so dont change this line
string current = Directory.GetCurrentDirectory();

ZipFile.ExtractToDirectory(current + "/tempApp.zip", current, true);

string filename = File.Exists(current + "/Forester.exe") ? current + "/Forester.exe" : current + "/Forester.Desktop.exe";

var process = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = filename
    }
};

process.Start();