using System;
using System.Collections.Generic;
using DocumentGenerator.Core.Services;
using DocumentGenerator.Data.Extensions;
using DocumentGenerator.UI.Models.Extensions;
using DocumentGenerator.UI.Models.Pages;
using DocumentGenerator.UI.ViewModels.Pages;
using Microsoft.Extensions.Logging;

namespace DocumentGenerator.UI.Services.WindowsNavigation;

public class WindowsNavigation : ISubscriber
{
    private readonly List<IDisposable> _subscriptions;
    private readonly ILogger<WindowsNavigation> _logger;
    private StartService _startService;
    private readonly IEnumerable<IManagerWindow> _managerWindowNotifier;
    private readonly IViewNavigation _navigation;

    public WindowsNavigation(ILogger<WindowsNavigation> logger, IEnumerable<IManagerWindow> managerWindowNotifier,
        IViewNavigation navigation)
    {
        _navigation = navigation;
        _logger = logger;
        _managerWindowNotifier = managerWindowNotifier;
        _subscriptions = new List<IDisposable>();
    }

    public void Subscribe()
    {
        foreach (var notifier in _managerWindowNotifier)
        {
            _subscriptions.Add(notifier.RedirectToView.Subscribe(OnRedirect));
        }
    }

    public void Unsubscribe()
    {
        _subscriptions.DisposeAndClear();
    }

    private void OnRedirect(ViewTypes targetView)
    {
        _logger.LogInformation($"Переход на страницу -> {targetView.ToDisplayString()}");
        
        /*
        if (_viewModels[ViewTypes.Layouts] is SelectLayoutsViewModel selectLayoutsViewModel)
        {
            switch (success)
            {
                /*case UserControlTypes.Edit:
                    //var editLayoutViewModel = new EditLayoutViewModel(selectLayoutsViewModel.NameEditLayout);
                    if (editLayoutViewModel is IUserControlsNotifier userControlsNotifier)
                    {
                        _subscriptions.Add(userControlsNotifier.RedirectToView.Subscribe(OnComplete));
                    }

                    _viewModels[UserControlTypes.Edit];
                    break;#1#

                case ViewTypes.Process:
                    var viewPath = _viewModels[ViewTypes.Path] as SelectPathsViewModel;
                    _startService = new StartService(selectLayoutsViewModel.GetCheckedNames(),
                        [viewPath.LocationDataText], viewPath.LocationFolderSaveText);
                    break;
            }
        }
        */
        
        _navigation.OnRedirect(targetView);
    }
}