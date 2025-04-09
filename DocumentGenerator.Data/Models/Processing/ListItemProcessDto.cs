using Avalonia.Media;

namespace DocumentGenerator.Data.Models.Processing;

public class ListItemProcessDto(IImmutableSolidColorBrush color, string text)
{
    public IImmutableSolidColorBrush Color { get; set; } = color;

    public string Text { get; set; } = text;
}