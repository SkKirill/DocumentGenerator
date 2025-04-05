using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.Reflection;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using DocumentGenerator.UI.ViewModels;
using DocumentGenerator.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace DocumentGenerator.UI;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var serviceProvider = ConfigureServices();

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
    
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Регистрация сервисов
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();

        // Логирование
        services.AddLogging(builder => builder
            .ClearProviders()
            .SetMinimumLevel(LogLevel.Trace)
            .AddNLog());

        return services.BuildServiceProvider();
    }
}

