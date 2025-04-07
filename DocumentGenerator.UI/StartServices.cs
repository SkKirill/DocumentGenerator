using System;
using DocumentGenerator.UI.Services;
using DocumentGenerator.UI.Services.WindowsNavigation;
using DocumentGenerator.UI.ViewModels;
using DocumentGenerator.UI.ViewModels.ExtensionPages;
using DocumentGenerator.UI.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace DocumentGenerator.UI;

/// <summary>
/// Сборка и конфигурирование контейнера для приложения
/// </summary>
public class StartServices
{
    public IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        AddLogging(services);

        AddViews(services);

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Добавление страниц
    /// </summary>
    private static void AddViews(ServiceCollection services)
    {
        services
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<SelectLayoutsViewModel>()
            .AddSingleton<ProcessingViewModel>()
            .AddSingleton<SelectPathsViewModel>()
            .AddSingleton<EditLayoutViewModel>()
            .AddSingleton<DataPathItemViewModel>();
        
        services
            .AddSingleton<IManagerWindow>(provider => provider.GetRequiredService<EditLayoutViewModel>())
            .AddSingleton<IManagerWindow>(provider => provider.GetRequiredService<SelectLayoutsViewModel>())
            .AddSingleton<IManagerWindow>(provider => provider.GetRequiredService<ProcessingViewModel>())
            .AddSingleton<IManagerWindow>(provider => provider.GetRequiredService<SelectPathsViewModel>())
            .AddSingleton<IViewNavigation>(provider => provider.GetRequiredService<MainWindowViewModel>());

        services
            .AddSingleton<WindowsNavigation>()
            .AddSingleton<ISubscriber>(provider => provider.GetRequiredService<WindowsNavigation>());
    }

    /// <summary>
    /// Добавление логирования в приложение 
    /// </summary>
    private static void AddLogging(ServiceCollection services)
    {
        services.AddLogging(builder => builder
            .ClearProviders()
            .SetMinimumLevel(LogLevel.Trace)
            .AddNLog());
    }
}