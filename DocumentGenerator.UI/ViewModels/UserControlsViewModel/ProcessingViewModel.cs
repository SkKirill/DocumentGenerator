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
    private readonly Subject<UserControlTypes> _redirectToView;
    public IObservable<UserControlTypes> RedirectToView => _redirectToView;
    public ReactiveCommand<Unit, Unit> ClearActionButton { get; }
    public ReactiveCommand<Unit, Unit> ContinueButton { get; }

    public ProcessingViewModel()
    {
        _redirectToView = new Subject<UserControlTypes>();
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
        _redirectToView.OnNext(UserControlTypes.Process);
    }

    private void RunGoBackAction()
    {
        // TODO: обработать завершение создание и возможное удаление того что уже создалось
        _redirectToView.OnNext(UserControlTypes.Layouts);
    }
}