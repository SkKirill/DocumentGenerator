using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DocumentGenerator.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MainWindow = DocumentGenerator.UI.Views.MainWindow;

namespace DocumentGenerator.UI;

/// <summary>
/// Главный класс приложения Avalonia.
/// Отвечает за инициализацию, загрузку ресурсов и запуск главного окна.
/// </summary>
public class App : Application
{
    /// <summary>
    /// Глобальный провайдер сервисов, настраивается при запуске.
    /// </summary>
    public static IServiceProvider ServiceProvider { get; set; } = null!;

    /// <summary>
    /// Загружает XAML-ресурсы приложения.
    /// </summary>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Вызывается после завершения инициализации платформы Avalonia.
    /// Используется для запуска основного окна и других зависимостей.
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        // Проверка на успешное конфигурирование контейнера
        if (ServiceProvider == null)
        {
            throw new InvalidOperationException("Сервис провайдер не был создан!");
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Получаем главное окно и ViewModel через DI
            try
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>()
                };
            }
            catch (Exception ex)
            {
                var logger = ServiceProvider.GetRequiredService<ILogger<App>>();
                logger.LogCritical(ex, "Ошибка при инициализации главного окна");
                throw;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}