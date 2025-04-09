using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DocumentGenerator.Data.Extensions;
using DocumentGenerator.Data.Models;
using DocumentGenerator.Data.Services.Interfaces;
using DocumentGenerator.UI.Models.Pages;
using DocumentGenerator.UI.Services;
using DocumentGenerator.UI.Services.Edit;
using DocumentGenerator.UI.Services.WindowsNavigation;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.Pages;

public class EditLayoutViewModel : ViewModelBase, IManagerWindow, ISubscriber
{
    public IObservable<ViewTypes> RedirectToView => _redirectToView;
    private readonly Subject<ViewTypes> _redirectToView;

    public ReactiveCommand<Unit, Unit> GoBackCommand => _goBackCommand.Value;
    public ReactiveCommand<Unit, Unit> ContinueCommand => _continueCommand.Value;

    public string NameLayout
    {
        get => _nameLayout;
        set => this.RaiseAndSetIfChanged(ref _nameLayout, value);
    }

    // Lazy команды
    private readonly Lazy<ReactiveCommand<Unit, Unit>> _goBackCommand;
    private readonly Lazy<ReactiveCommand<Unit, Unit>> _continueCommand;
    private readonly List<IDisposable> _subscriptions;
    private readonly ILogger<EditLayoutViewModel> _logger;
    private readonly IEnumerable<ILayoutNameNotifier> _layoutNameNotifiers;
    private string _nameLayout;

    public EditLayoutViewModel(
        ILogger<EditLayoutViewModel> logger,
        IEnumerable<ILayoutNameNotifier> layoutNameNotifiers)
    {
        _subscriptions = new List<IDisposable>();
        _logger = logger;
        _layoutNameNotifiers = layoutNameNotifiers;
        _nameLayout = string.Empty;
        _redirectToView = new Subject<ViewTypes>();
        _goBackCommand = new(() =>
            ReactiveCommand.CreateFromTask(RunGoBackAction, outputScheduler: RxApp.MainThreadScheduler));
        _continueCommand = new(() =>
            ReactiveCommand.CreateFromTask(RunContinue, outputScheduler: RxApp.MainThreadScheduler));
    }

    private async Task RunContinue()
    {
        // TODO: выполнить сохранение созданных макетов
        _redirectToView.OnNext(ViewTypes.Layouts);
    }

    private async Task RunGoBackAction()
    {
        // TODO: спросить сохранять или удалять выбранные макеты создания файлов
        _redirectToView.OnNext(ViewTypes.Layouts);
    }

    public void Subscribe()
    {
        foreach (var notifier in _layoutNameNotifiers)
        {
            _subscriptions.Add(notifier.NameEditLayout.Subscribe(OnEditItem));
        }
    }

    public void Unsubscribe()
    {
        _subscriptions.DisposeAndClear();
    }

    private void OnEditItem(string name)
    {
        _logger.LogInformation($"На редактирование пришел макет: {name}");
        NameLayout = name;
    }
}