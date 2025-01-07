using System;
using DocumentGenerator.UI.ViewModels;
using ReactiveUI;

namespace DocumentGenerator.UI.Models;

public class ListItemModel : ReactiveObject
{
    private bool _isChecked;
    public bool IsChecked
    {
        get => _isChecked;
        set => this.RaiseAndSetIfChanged(ref _isChecked, value);
    }
    private string _text;
    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }
    public Action EditAction { get; set; }

    public ListItemModel(string text, Action editAction)
    {
        Text = text;
        EditAction = editAction;
        IsChecked = false;
    }
}