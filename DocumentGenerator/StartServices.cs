using DocumentGenerator.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using DocumentGenerator.Data.Services;
using DocumentGenerator.Data.Services.DataBase.Repositories;
using DocumentGenerator.UI.Models;
using DocumentGenerator.UI.Services;
using DocumentGenerator.UI.Services.Edit;
using DocumentGenerator.UI.Services.WindowsNavigation;
using DocumentGenerator.UI.ViewModels;
using DocumentGenerator.UI.ViewModels.ExtensionPages;
using DocumentGenerator.UI.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace DocumentGenerator;

/// <summary>
/// Сборка и конфигурирование контейнера для приложения
/// </summary>
public static class StartServices
{
    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        AddLogging(services);
        AddLayouts(services);
        AddViews(services);

        services
            .AddStartServices();

        return services.BuildServiceProvider();
    }

    private static void AddLayouts(IServiceCollection services)
    {
        using var layoutRepository = new LayoutRepository();
        layoutRepository.UseContext(new DatabaseContext());

        var layoutNames = layoutRepository.GetLayoutsAsync().Result
            .Select(item => item.Name).ToList();

        foreach (var name in layoutNames)
        {
            services
                .AddSingleton(provider => new ListLayoutsModel(name))
                .AddSingleton<ILayoutNameNotifier>(provider => provider.GetRequiredService<ListLayoutsModel>());
        }
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
            .AddSingleton<ISubscriber>(provider => provider.GetRequiredService<WindowsNavigation>())
            .AddSingleton<ISubscriber>(provider => provider.GetRequiredService<SelectLayoutsViewModel>())
            .AddSingleton<ISubscriber>(provider => provider.GetRequiredService<EditLayoutViewModel>())
            .AddSingleton<ISubscriber>(provider => provider.GetRequiredService<ProcessingViewModel>());
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

    public static IServiceCollection AddStartServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<StartProcess>()
            .AddSingleton<IWriteProcessingNotifier>(provider => provider.GetRequiredService<StartProcess>());
    }
}