using System;
using System.IO;
using Avalonia;
using Avalonia.ReactiveUI;

namespace Forester.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
#if RELEASE
        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            File.WriteAllText("./Forester-errorlog.txt", ex.Message);
        }
#else
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
#endif
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UseReactiveUI()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

}
