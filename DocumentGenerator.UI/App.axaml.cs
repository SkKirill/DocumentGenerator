using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DocumentGenerator.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MainWindow = DocumentGenerator.UI.Views.MainWindow;

namespace DocumentGenerator.UI;

public class App : Application
{
    public static IServiceProvider ServiceProvider { get; set; }

    private ILogger<App> _logger;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Получаем логгер через DI
        _logger = ServiceProvider.GetRequiredService<ILogger<App>>();
        _logger.LogInformation("Приложение инициализируется...");

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _logger.LogInformation("Настраиваем главное окно...");
            
            desktop.MainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            desktop.MainWindow.DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>();
        }

        base.OnFrameworkInitializationCompleted();

        _logger.LogInformation("Приложение успешно запущено.");
    }
}