using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.Reflection;
using System.Threading;
using Avalonia.Controls;
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
        BuildAvaloniaApp().SetupWithClassicDesktopLifetime(args);
    }

    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
    }
    
    private static void AppMain(Application app, string[] args)
    {
        var tokenSource = new CancellationTokenSource();

        IServiceProvider provider = new ServiceCollection()
            .AddLogging(builder => builder
                .ClearProviders()
                .SetMinimumLevel(LogLevel.Trace)
                .AddNLog())
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<MainWindow>().BuildServiceProvider();
        
        var mainWindow = provider.GetRequiredService<MainWindow>();
        
        mainWindow.Show();
        
        app.Run(tokenSource.Token);
    }
}