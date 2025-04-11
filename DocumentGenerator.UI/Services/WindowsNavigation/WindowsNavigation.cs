using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using DocumentGenerator.Data.Extensions;
using DocumentGenerator.Data.Models.Processing;
using DocumentGenerator.UI.Models.Extensions;
using DocumentGenerator.UI.Models.Pages;
using Microsoft.Extensions.Logging;

namespace DocumentGenerator.UI.Services.WindowsNavigation;

public class WindowsNavigation : IStarterNotifier
{
    public IObservable<bool> Start => _starter;
    
    
    private readonly List<IDisposable> _subscriptions;
    private readonly ILogger<WindowsNavigation> _logger;
    private readonly IEnumerable<IWindowManager> _managerWindowNotifier;
    private readonly IViewNavigation _navigation;
    private readonly Subject<bool> _starter;

    public WindowsNavigation(ILogger<WindowsNavigation> logger, IEnumerable<IWindowManager> managerWindowNotifier,
        IViewNavigation navigation)
    {
        _starter = new Subject<bool>();
        _navigation = navigation;
        _logger = logger;
        _managerWindowNotifier = managerWindowNotifier;
        _subscriptions = new List<IDisposable>();
        Subscribe();
    }

    public void Subscribe()
    {
        foreach (var notifier in _managerWindowNotifier)
        {
            _subscriptions.Add(notifier.RedirectToView.Subscribe(OnRedirect));
        }
    }

    private void OnRedirect(ViewTypes targetView)
    {
        _logger.LogInformation($"Переход на страницу -> {targetView.ToDisplayString()}");
        switch (targetView)
        {
            case ViewTypes.Layouts:
                break;
            case ViewTypes.Path:
                break;
            case ViewTypes.Edit:
                break;
            case ViewTypes.Process:
                _starter.OnNext(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(targetView), targetView, null);
        }
        _navigation.OnRedirect(targetView);
    }
}