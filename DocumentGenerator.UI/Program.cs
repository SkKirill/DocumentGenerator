using Avalonia;
using Avalonia.ReactiveUI;
using System;
using DocumentGenerator.Core.Services;
using DocumentGenerator.Data.Models;
using DocumentGenerator.Data.Models.Processing;
using DocumentGenerator.Data.Services.Interfaces;
using DocumentGenerator.UI.Services;
using DocumentGenerator.UI.Services.WindowsNavigation;
using DocumentGenerator.UI.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace DocumentGenerator.UI;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Загружаем конфигурацию NLog
        LogManager.Setup().LoadConfigurationFromFile("nlog.config", optional: false);


        var lineSeparator = new string('-', 100);
        LogManager.GetCurrentClassLogger().Info(lineSeparator);
        LogManager.GetCurrentClassLogger().Info("Запуск приложения...");
        LogManager.GetCurrentClassLogger().Info(lineSeparator);

        // Создаем сервис-провайдер
        var provider = StartServices.ConfigureServices();
        
        var startProcess = provider.GetRequiredService<StartProcess>();
        startProcess.StarterNotifier = provider.GetRequiredService<IStarterNotifier>();
        
        var subscribers = provider.GetServices<ISubscriber>();
        foreach (var subscriber in subscribers)
        {
            subscriber.Subscribe();
        }

        // Запускаем Avalonia-приложение
        BuildAvaloniaApp(provider)
            .StartWithClassicDesktopLifetime(args);
    }

    private static AppBuilder BuildAvaloniaApp(IServiceProvider serviceProvider)
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI()
            .AfterSetup(_ => { App.ServiceProvider = serviceProvider; });
    }
}