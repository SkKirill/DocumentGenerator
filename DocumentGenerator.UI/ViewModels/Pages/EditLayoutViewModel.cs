using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Avalonia.Layout;
using DocumentGenerator.Core.Services.ReaderTables;
using DocumentGenerator.Data.Extensions;
using DocumentGenerator.Data.Models;
using DocumentGenerator.Data.Services.Interfaces;
using DocumentGenerator.UI.Models.Pages;
using DocumentGenerator.UI.Services;
using DocumentGenerator.UI.Services.Edit;
using DocumentGenerator.UI.Services.WindowsNavigation;
using DynamicData;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.Pages;

public class EditLayoutViewModel : ViewModelBase, IWindowNavigation, ISubscriber
{
    public IObservable<ViewTypes> RedirectToView => _redirectToView;
    private readonly Subject<ViewTypes> _redirectToView;

    public ReactiveCommand<Unit, Unit> GoBackCommand => _goBackCommand.Value;
    public ReactiveCommand<Unit, Unit> ContinueCommand => _continueCommand.Value;
    public ReactiveCommand<Unit, Unit> GoSettingsCommand => _goSettings.Value;


    public string NameLayout
    {
        get => _nameLayout;
        set => this.RaiseAndSetIfChanged(ref _nameLayout, value);
    }

    public ObservableCollection<string> ColumnNames { get; set; }
    
    // Lazy команды
    private readonly Lazy<ReactiveCommand<Unit, Unit>> _goBackCommand;
    private readonly Lazy<ReactiveCommand<Unit, Unit>> _continueCommand;
    private readonly Lazy<ReactiveCommand<Unit, Unit>> _goSettings;
    private readonly List<IDisposable> _subscriptions;
    private readonly IEnumerable<ILayoutNameNotifier> _layoutNameNotifiers;
    private readonly ILogger<EditLayoutViewModel> _logger;
    private readonly IReadManager _readManager;
    private string _nameLayout;

    public EditLayoutViewModel(
        ILogger<EditLayoutViewModel> logger,
        IEnumerable<ILayoutNameNotifier> layoutNameNotifiers,
        IReadManager readManager)
    {
        _readManager = readManager;
        ColumnNames = [];
        _subscriptions = new List<IDisposable>();
        _logger = logger;
        _layoutNameNotifiers = layoutNameNotifiers;
        _nameLayout = string.Empty;
        _redirectToView = new Subject<ViewTypes>();
        _goBackCommand = new(() =>
            ReactiveCommand.CreateFromTask(RunGoBackAction, outputScheduler: RxApp.MainThreadScheduler));
        _continueCommand = new(() =>
            ReactiveCommand.CreateFromTask(RunContinue, outputScheduler: RxApp.MainThreadScheduler));
        _goSettings = new(() =>
            ReactiveCommand.CreateFromTask(RunGoSettings, outputScheduler: RxApp.MainThreadScheduler));
    }

    private async Task RunContinue()
    {
        // TODO: выполнить сохранение созданных макетов
        _redirectToView.OnNext(ViewTypes.Layouts);
    }
    
    private async Task RunGoSettings()
    {
        // TODO: переход на страницу с настройками
        _redirectToView.OnNext(ViewTypes.Settings);
    }

    private async Task RunGoBackAction()
    {
        // TODO: не сохранять изменения
        _redirectToView.OnNext(ViewTypes.Layouts);
    }

    public void Subscribe()
    {
        foreach (var notifier in _layoutNameNotifiers)
        {
            _subscriptions.Add(notifier.NameEditLayout.Subscribe(OnStartEditingItem));
        }
    }

    public void Unsubscribe()
    {
        _subscriptions.DisposeAndClear();
    }

    private void OnStartEditingItem(string name)
    {
        _logger.LogInformation($"На редактирование пришел макет: {name}");
        NameLayout = name;
        ColumnNames.AddRange(_readManager.CreateListColumns().Select(item => item.ColumnName));
    }
}