using System;
using System.Reactive;
using System.Reactive.Subjects;
using DocumentGenerator.UI.Services;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.UserControlsViewModel;

public class EditLayoutViewModel : ViewModelBase, IUserControlsNotifier
{
    private readonly Subject<UserControlTypes> _redirectToView;
    public IObservable<UserControlTypes> RedirectToView => _redirectToView;
    public ReactiveCommand<Unit, Unit> ClearActionButton { get; }
    public ReactiveCommand<Unit, Unit> ContinueButton { get; }

    public string NameLayout
    {
        get => _nameLayout;
        set => this.RaiseAndSetIfChanged(ref _nameLayout, value);
    }
    
    private string _nameLayout;
    
    public EditLayoutViewModel(string nameLayout)
    {
        NameLayout = nameLayout;
        _redirectToView = new Subject<UserControlTypes>();
        ClearActionButton = ReactiveCommand.Create(RunClearAction);
        ContinueButton = ReactiveCommand.Create(RunContinue);
    }
    
    private void RunContinue()
    {
        // TODO: выполнить сохранение созданных макетов
        _redirectToView.OnNext(UserControlTypes.Layouts);
    }

    private void RunClearAction()
    {
        // TODO: спросить сохранять или удалять выбранные макеты создания файлов
        _redirectToView.OnNext(UserControlTypes.Layouts);
    }

}