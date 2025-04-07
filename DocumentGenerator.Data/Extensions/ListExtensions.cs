namespace DocumentGenerator.Data.Extensions;

public static class ListExtensions
{
    public static void DisposeAndClear(this List<IDisposable> disposables)
    {
        disposables.ForEach(disposable => disposable.Dispose());
        disposables.Clear();
    }
}