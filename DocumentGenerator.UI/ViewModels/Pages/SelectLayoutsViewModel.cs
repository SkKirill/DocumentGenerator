using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DocumentGenerator.Data.Extensions;
using DocumentGenerator.Data.Models.DataUi;
using DocumentGenerator.Data.Services.Interfaces;
using DocumentGenerator.UI.Models;
using DocumentGenerator.UI.Models.Pages;
using DocumentGenerator.UI.Services.Edit;
using DocumentGenerator.UI.Services.WindowsNavigation;
using DynamicData;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.Pages;

public class SelectLayoutsViewModel : ViewModelBase, IManagerWindow, ISubscriber
{
    public IObservable<ViewTypes> RedirectToView => _redirectToView;
    public ObservableCollection<ListLayoutsModel> ListLayouts { get; set; }
    public ReactiveCommand<Unit, Unit> GoBackActionButton => _goBackCommand.Value;
    public ReactiveCommand<Unit, Unit> ContinueButton => _continueCommand.Value;

    private readonly Subject<ViewTypes> _redirectToView;
    private readonly List<IDisposable> _subscriptions;
    private readonly ILogger<SelectLayoutsViewModel> _logger;
    private readonly IEnumerable<ILayoutNameNotifier> _layoutNameNotifiers;
    private readonly InputData _inputData; 
    
    // Lazy команды
    private readonly Lazy<ReactiveCommand<Unit, Unit>> _goBackCommand;
    private readonly Lazy<ReactiveCommand<Unit, Unit>> _continueCommand;

    public SelectLayoutsViewModel(
        ILogger<SelectLayoutsViewModel> logger,
        IEnumerable<ILayoutNameNotifier> layoutNameNotifiers,
        IEnumerable<ListLayoutsModel> listLayouts,
        InputData inputData)
    {
        _inputData = inputData;
        _layoutNameNotifiers = layoutNameNotifiers;
        _logger = logger;
        _subscriptions = new List<IDisposable>();
        _redirectToView = new Subject<ViewTypes>();

        _goBackCommand = new(() =>
            ReactiveCommand.CreateFromTask(RunGoBackAction, outputScheduler: RxApp.MainThreadScheduler));
        _continueCommand = new(() =>
            ReactiveCommand.CreateFromTask(RunContinue, outputScheduler: RxApp.MainThreadScheduler));

        var names = _layoutNameNotifiers.ToList().Select(item => item.NameLayout).ToList();

        ListLayouts = [];
        ListLayouts.AddRange(listLayouts);
    }

    private async Task RunContinue()
    {
        if (ListLayouts.Any(item => item.IsChecked))
        {
            _redirectToView.OnNext(ViewTypes.Process);
            _logger.LogInformation($"Выбраны макеты для создания: {string.Join(
                " ",
                ListLayouts.Select(item => item.NameLayout))}");
                _inputData.ListLayouts = ListLayouts.Select(item => item.NameLayout).ToList();
            return;
        }

        _logger.LogWarning("Переход на следующую страницу не будет выполнен -> не выбран ни один макет");
    }

    private async Task RunGoBackAction()
    {
        _redirectToView.OnNext(ViewTypes.Path);
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
        _redirectToView.OnNext(ViewTypes.Edit);
    }
}