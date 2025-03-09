using System;
using System.Reactive;
using System.Reactive.Subjects;
using DocumentGenerator.UI.Services;
using ReactiveUI;

namespace DocumentGenerator.UI.Models;

public class ListLayoutsModel : ReactiveObject, IEditLayoutStartNotifier
{
    public IObservable<string> NameEditLayout => _nameEditLayout;
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
    public ReactiveCommand<Unit, Unit> EditAction { get; }
    
    private string _nameLayout;
    private bool _isChecked;

    public ListLayoutsModel(string nameLayout)
    {
        NameLayout = nameLayout;
        IsChecked = false;
        EditAction = ReactiveCommand.Create(EditActionCommand);
        _nameEditLayout = new Subject<string>();
    }

    private void EditActionCommand()
    {
        _nameEditLayout.OnNext(NameLayout);
    }

}