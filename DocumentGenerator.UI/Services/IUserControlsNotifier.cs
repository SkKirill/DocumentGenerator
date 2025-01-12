using System;

namespace DocumentGenerator.UI.Services;

public interface IUserControlsNotifier
{
    public IObservable<bool> CompleteView { get; }
}