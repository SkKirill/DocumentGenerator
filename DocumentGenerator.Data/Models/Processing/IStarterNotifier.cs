namespace DocumentGenerator.Data.Models.Processing;

public interface IStarterNotifier
{
    public IObservable<bool> Start { get; }
}