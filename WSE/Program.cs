using Avalonia;
using Avalonia.ReactiveUI;
using Serilog;
using System;

namespace WSE;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("./logs/log-.txt",
                rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .WriteTo.Trace()
            .MinimumLevel.Debug()
            .CreateLogger();

        Log.Information("The global logger has been configured");

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}
