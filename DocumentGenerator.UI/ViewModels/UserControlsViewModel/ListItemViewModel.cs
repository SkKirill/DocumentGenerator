using System;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.UserControlsViewModel;

public class ListItemViewModel : ViewModelBase
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

    public ListItemViewModel(string text, Action editAction)
    {
        Text = text;
        EditAction = editAction;
        IsChecked = false;
    }
}