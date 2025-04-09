using System;

namespace DocumentGenerator.UI.Services.Edit;

public interface ILayoutNameNotifier
{
    public string NameLayout { get; set; }
    public IObservable<string> NameEditLayout { get; }
}