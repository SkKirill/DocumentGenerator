using System;
using DocumentGenerator.UI.ViewModels;
using DocumentGenerator.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace DocumentGenerator.UI;

public class StartServices
{
    public IServiceProvider ConfigureServices()
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