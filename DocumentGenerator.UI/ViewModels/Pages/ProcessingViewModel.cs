using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Subjects;
using Avalonia.Media;
using DocumentGenerator.UI.Models;
using DocumentGenerator.UI.Models.Pages;
using DocumentGenerator.UI.Services;
using DocumentGenerator.UI.Services.WindowsNavigation;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.Pages;

public class ProcessingViewModel : ViewModelBase, IManagerWindow
{
    public ObservableCollection<ListItemProcessingModel> ProcessingText { get; set; }
    private readonly Subject<ViewTypes> _redirectToView;
    public IObservable<ViewTypes> RedirectToView => _redirectToView;
    public ReactiveCommand<Unit, Unit> ClearActionButton { get; }
    public ReactiveCommand<Unit, Unit> ContinueButton { get; }
    
    public ProcessingViewModel()
    {
        _redirectToView = new Subject<ViewTypes>();
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
        
        ClearActionButton = ReactiveCommand.Create(RunGoBackAction);
        ContinueButton = ReactiveCommand.Create(RunContinue);
    }
    
    private void RunContinue()
    {
        _redirectToView.OnNext(ViewTypes.Process);
    }

    private void RunGoBackAction()
    {
        // TODO: обработать завершение создание и возможное удаление того что уже создалось
        _redirectToView.OnNext(ViewTypes.Layouts);
    }
    
}