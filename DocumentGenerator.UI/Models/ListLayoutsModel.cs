using System;
using ReactiveUI;

namespace DocumentGenerator.UI.Models;

public class ListLayoutsModel : ReactiveObject
{
    public bool IsChecked
    {
        get => _isChecked;
        set => this.RaiseAndSetIfChanged(ref _isChecked, value);
    }
    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }
    public Action EditAction { get; set; }
    
    private string _text;
    private bool _isChecked;

    public ListLayoutsModel(string text, Action editAction)
    {
        Text = text;
        EditAction = editAction;
        IsChecked = false;
    }
}