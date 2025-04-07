using DocumentGenerator.UI.Models.Pages;

namespace DocumentGenerator.UI.Models.Extensions;

public static class ViewTypesExtensions
{
    public static string ToDisplayString(this ViewTypes type) =>
        type switch
        {
            ViewTypes.Layouts => "Выбор шаблонов",
            ViewTypes.Path => "Выбор путей к данным",
            ViewTypes.Edit => "Редактирование шаблона",
            ViewTypes.Process => "Процесс создания документов",
            _ => type.ToString()
        };
}
