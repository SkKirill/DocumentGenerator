using System;
using DocumentGenerator.UI.Models.Pages;
using DocumentGenerator.UI.ViewModels.Pages;

namespace DocumentGenerator.UI.Services.WindowsNavigation;

/// <summary>
/// Интерфейс выкидывающий событие с типом страницы, которую нужно показать пользователю
/// </summary>
public interface IWindowNavigation
{
    IObservable<ViewTypes> RedirectToView { get; }
}