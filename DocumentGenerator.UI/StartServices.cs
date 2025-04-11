using System;
using System.Linq;
using DocumentGenerator.Core;
using DocumentGenerator.Data.Models;
using DocumentGenerator.Data.Models.DataUi;
using DocumentGenerator.Data.Models.Processing;
using DocumentGenerator.Data.Services;
using DocumentGenerator.Data.Services.DataBase.Repositories;
using DocumentGenerator.Data.Services.Interfaces;
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

namespace DocumentGenerator.UI;

public static class StartServices
{
    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services
            .AddLogging()
            .AddCore()
            .AddLayouts()
            .AddData()
            .AddViews();

        return services.BuildServiceProvider();
    }

    private static IServiceCollection AddData(this IServiceCollection services)
    {
        return services
            .AddSingleton<InputData>();
    }

    private static IServiceCollection AddLayouts(this IServiceCollection services)
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

        return services;
    }

    /// <summary>
    /// Добавление страниц
    /// </summary>
    private static IServiceCollection AddViews(this IServiceCollection services)
    {
        services
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<SelectLayoutsViewModel>()
            .AddSingleton<ProcessingViewModel>()
            .AddSingleton<SelectPathsViewModel>()
            .AddSingleton<EditLayoutViewModel>()
            .AddSingleton<DataPathItemViewModel>()
            .AddSingleton<PageSettingsViewModel>();

        services
            .AddSingleton<IWindowNavigation>(provider => provider.GetRequiredService<EditLayoutViewModel>())
            .AddSingleton<IWindowNavigation>(provider => provider.GetRequiredService<SelectLayoutsViewModel>())
            .AddSingleton<IWindowNavigation>(provider => provider.GetRequiredService<ProcessingViewModel>())
            .AddSingleton<IWindowNavigation>(provider => provider.GetRequiredService<SelectPathsViewModel>())
            .AddSingleton<IWindowNavigation>(provider => provider.GetRequiredService<PageSettingsViewModel>())
            .AddSingleton<IViewNavigation>(provider => provider.GetRequiredService<MainWindowViewModel>());

        services
            .AddSingleton<WindowsNavigationManager>()
            .AddSingleton<IStarterNotifier>(provider => provider.GetRequiredService<WindowsNavigationManager>())
            .AddSingleton<ISubscriber>(provider => provider.GetRequiredService<SelectLayoutsViewModel>())
            .AddSingleton<ISubscriber>(provider => provider.GetRequiredService<ProcessingViewModel>())
            .AddSingleton<ISubscriber>(provider => provider.GetRequiredService<PageSettingsViewModel>())
            .AddSingleton<ISubscriber>(provider => provider.GetRequiredService<EditLayoutViewModel>());
        
        return services;
    }

    /// <summary>
    /// Добавление логирования в приложение 
    /// </summary>
    private static IServiceCollection AddLogging(this IServiceCollection services)
    {
        return services.AddLogging(builder => builder
            .ClearProviders()
            .SetMinimumLevel(LogLevel.Trace)
            .AddNLog());
    }
}