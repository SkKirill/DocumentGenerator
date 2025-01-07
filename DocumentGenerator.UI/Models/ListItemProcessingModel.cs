using Avalonia.Media;
using ReactiveUI;

namespace DocumentGenerator.UI.Models;

public class ListItemProcessingModel : ReactiveObject
{
    private IImmutableSolidColorBrush _color;
    private string _text;
    
    public IImmutableSolidColorBrush Color
    {
        get => _color;
        set => this.RaiseAndSetIfChanged(ref _color, value);
    }
    
    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }

    public ListItemProcessingModel(IImmutableSolidColorBrush color, string text)
    {
        _color = color;
        _text = text;
    }    
}