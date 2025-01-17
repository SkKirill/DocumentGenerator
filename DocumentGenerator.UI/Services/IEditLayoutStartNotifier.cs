using System;

namespace DocumentGenerator.UI.Services;

public interface IEditLayoutStartNotifier
{
    public IObservable<string> NameEditLayout { get; }
}