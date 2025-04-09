namespace DocumentGenerator.Data.Models.Processing;

public interface IWriteProcessingNotifier
{
    public IObservable<ListItemProcessDto> WriteText { get; }
}