using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Subjects;
using Avalonia.Media;
using DocumentGenerator.UI.Models;
using DocumentGenerator.UI.Services;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.UserControlsViewModel;

public class ProcessingViewModel : ViewModelBase, IUserControlsNotifier
{
    public ObservableCollection<ListItemProcessingModel> ProcessingText { get; set; }
    private Subject<bool> _completeView;
    public IObservable<bool> CompleteView => _completeView;
    public ReactiveCommand<Unit, Unit> ClearActionButton { get; }
    public ReactiveCommand<Unit, Unit> ContinueButton { get; }
    
    public ProcessingViewModel()
    {
        _completeView = new Subject<bool>();
        ProcessingText = new ObservableCollection<ListItemProcessingModel>();

        ProcessingText.Add(new ListItemProcessingModel(Brushes.Red, "Диплом готов"));
        ProcessingText.Add(new ListItemProcessingModel(Brushes.Green, "Пример готов"));
        ProcessingText.Add(new ListItemProcessingModel(Brushes.Red, "Сертификат готов"));
        ProcessingText.Add(new ListItemProcessingModel(Brushes.Green, "Ошибка при создании"));
        ProcessingText.Add(new ListItemProcessingModel(Brushes.Red, "Сертификат не готов"));
        ProcessingText.Add(new ListItemProcessingModel(Brushes.Red, "Сертификат не готов"));
        ProcessingText.Add(new ListItemProcessingModel(Brushes.Red, "Сертификат не готов"));
        ProcessingText.Add(new ListItemProcessingModel(Brushes.Green, "Ошибка при создании"));
        ProcessingText.Add(new ListItemProcessingModel(Brushes.Green, "Ошибка при создании"));
        ProcessingText.Add(new ListItemProcessingModel(Brushes.Green, "Диплом готов"));
        
        ClearActionButton = ReactiveCommand.Create(RunClearAction);
        ContinueButton = ReactiveCommand.Create(RunContinue);
    }
    
    private void RunContinue()
    {
        _completeView.OnNext(true);
    }

    private void RunClearAction()
    {
        _completeView.OnNext(false);
        // TODO: обработка нажатия в окошко поиска директории
    }
    
}