using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DocumentGenerator.UI.Services;
using DocumentGenerator.UI.Services.Edit;
using ReactiveUI;

namespace DocumentGenerator.UI.Models;

public class ListLayoutsModel : ReactiveObject, ILayoutNameNotifier
{
    public IObservable<string> NameEditLayout => _nameEditLayout;
    public ReactiveCommand<Unit, Unit> EditAction => _editAction.Value;
    public bool IsChecked
    {
        get => _isChecked;
        set => this.RaiseAndSetIfChanged(ref _isChecked, value);
    }
    public string NameLayout
    {
        get => _nameLayout;
        set => this.RaiseAndSetIfChanged(ref _nameLayout, value);
    }

    private readonly Subject<string> _nameEditLayout;
    private readonly Lazy<ReactiveCommand<Unit, Unit>> _editAction;
    
    private string _nameLayout;
    private bool _isChecked;

    public ListLayoutsModel(string nameLayout)
    {
        _nameLayout = nameLayout;
        IsChecked = false;
        _editAction = new(() =>
            ReactiveCommand.CreateFromTask(EditActionCommand, outputScheduler: RxApp.MainThreadScheduler));
        _nameEditLayout = new Subject<string>();
    }

    private async Task EditActionCommand()
    {
        _nameEditLayout.OnNext(NameLayout);
    }
}