using System;
using System.Reactive;
using System.Reactive.Subjects;
using DocumentGenerator.UI.Services;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.UserControlsViewModel;

public class EditLayoutViewModel : ViewModelBase, IUserControlsNotifier
{
    private Subject<bool> _completeView;
    public IObservable<bool> CompleteView => _completeView;
    public ReactiveCommand<Unit, Unit> ClearActionButton { get; }
    public ReactiveCommand<Unit, Unit> ContinueButton { get; }
    
    public EditLayoutViewModel()
    {
        _completeView = new Subject<bool>();
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