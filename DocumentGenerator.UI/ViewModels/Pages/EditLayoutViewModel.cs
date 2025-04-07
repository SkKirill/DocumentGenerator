using System;
using System.Reactive;
using System.Reactive.Subjects;
using DocumentGenerator.UI.Models.Pages;
using DocumentGenerator.UI.Services.WindowsNavigation;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.Pages;

public class EditLayoutViewModel : ViewModelBase, IManagerWindow
{
    public IObservable<ViewTypes> RedirectToView => _redirectToView;
    private readonly Subject<ViewTypes> _redirectToView;
    
    public ReactiveCommand<Unit, Unit> ClearActionButton { get; }
    public ReactiveCommand<Unit, Unit> ContinueButton { get; }

    public string NameLayout
    {
        get => _nameLayout;
        set => this.RaiseAndSetIfChanged(ref _nameLayout, value);
    }
    
    private string _nameLayout;
    
    public EditLayoutViewModel()
    {
        NameLayout = string.Empty;
        _redirectToView = new Subject<ViewTypes>();
        ClearActionButton = ReactiveCommand.Create(RunClearAction);
        ContinueButton = ReactiveCommand.Create(RunContinue);
    }
    
    private void RunContinue()
    {
        // TODO: выполнить сохранение созданных макетов
        _redirectToView.OnNext(ViewTypes.Layouts);
    }

    private void RunClearAction()
    {
        // TODO: спросить сохранять или удалять выбранные макеты создания файлов
        _redirectToView.OnNext(ViewTypes.Layouts);
    }

}