using System;
using DocumentGenerator.UI.ViewModels.UserControlsViewModel;

namespace DocumentGenerator.UI.Services;

public interface IUserControlsNotifier
{
    public IObservable<UserControlTypes> RedirectToView { get; }
}