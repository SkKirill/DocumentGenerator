using System;

namespace DocumentGenerator.UI.ViewModels;

public interface IUserControlsNotifier
{
    public IObservable<bool> CompleteView { get; }
}