using Avalonia;
using Avalonia.ReactiveUI;
using System;
using NLog;

namespace DocumentGenerator.UI;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Загружаем NLog конфиг
        LogManager.Setup().LoadConfigurationFromFile("nlog.config", optional: false);
        
        var serviceProvider = new StartServices().ConfigureServices();

        BuildAvaloniaApp(serviceProvider)
            .StartWithClassicDesktopLifetime(args);
    }

    private static AppBuilder BuildAvaloniaApp(IServiceProvider serviceProvider)
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI()
            .AfterSetup(_ =>
            {
                // Передаем сервис-провайдер в App
                App.ServiceProvider = serviceProvider;
            });
    }
}

