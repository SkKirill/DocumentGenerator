using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using DocumentGenerator.UI.ViewModels;

namespace DocumentGenerator.UI;

/// <summary>
/// Используется Avalonia для автоматического связывания ViewModel с соответствующим View.
/// </summary>
public class ViewLocator : IDataTemplate
{
    /// <summary>
    /// Создает экземпляр View и связывает его с ViewModel.
    /// </summary>
    /// <param name="data">Экземпляр ViewModel.</param>
    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type is not null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Не найдено: " + name };
    }

    /// <summary>
    /// Проверяет, соответствует ли объект типу ViewModelBase.
    /// </summary>
    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}