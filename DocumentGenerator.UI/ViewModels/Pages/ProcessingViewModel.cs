using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DocumentGenerator.Data.Extensions;
using DocumentGenerator.Data.Models;
using DocumentGenerator.Data.Models.Processing;
using DocumentGenerator.Data.Services.Interfaces;
using DocumentGenerator.UI.Models;
using DocumentGenerator.UI.Models.Pages;
using DocumentGenerator.UI.Services;
using DocumentGenerator.UI.Services.WindowsNavigation;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.Pages;

public class ProcessingViewModel : ViewModelBase, IManagerWindow, ISubscriber
{
    public ObservableCollection<ListItemProcessingModel> ProcessingText { get; set; }
    private readonly Subject<ViewTypes> _redirectToView;
    public IObservable<ViewTypes> RedirectToView => _redirectToView;
    public ReactiveCommand<Unit, Unit> GoBackCommand => _goBackCommand.Value;
    public ReactiveCommand<Unit, Unit> ContinueCommand => _continueCommand.Value;
    
    private readonly ILogger<ProcessingViewModel> _logger;
    private readonly Lazy<ReactiveCommand<Unit, Unit>> _goBackCommand;
    private readonly Lazy<ReactiveCommand<Unit, Unit>> _continueCommand;
    private readonly List<IDisposable> _subscriptions;
    private readonly IEnumerable<IWriteProcessingNotifier> _writeProcessingNotifiers;
    
    public ProcessingViewModel(
        ILogger<ProcessingViewModel> logger,
        IEnumerable<IWriteProcessingNotifier> writeProcessingNotifier)
    {
        _writeProcessingNotifiers = writeProcessingNotifier;
        _subscriptions = new List<IDisposable>();
        _logger = logger;
        _redirectToView = new Subject<ViewTypes>();
        ProcessingText = new ObservableCollection<ListItemProcessingModel>();
        
        _goBackCommand = new(() =>
            ReactiveCommand.CreateFromTask(RunGoBackAction, outputScheduler: RxApp.MainThreadScheduler));
        _continueCommand = new(() =>
            ReactiveCommand.CreateFromTask(RunContinue, outputScheduler: RxApp.MainThreadScheduler));
    }
    
    private async Task RunContinue()
    {
        // TODO: дальше уже некуда
        _redirectToView.OnNext(ViewTypes.Process);
    }

    private async Task RunGoBackAction()
    {
        // TODO: обработать завершение создание и возможное удаление того что уже создалось
        _redirectToView.OnNext(ViewTypes.Layouts);
    }

    public void Subscribe()
    {
        foreach (var notifier in _writeProcessingNotifiers)
        {
            _subscriptions.Add(notifier.WriteText.Subscribe(OnAddText));
        }
    }

    public void Unsubscribe()
    {
        _subscriptions.DisposeAndClear();
    }

    private void OnAddText(ListItemProcessDto textItem)
    {
        ProcessingText.Add(new ListItemProcessingModel(textItem.Color, textItem.Text));
    }
}