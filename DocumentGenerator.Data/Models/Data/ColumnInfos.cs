namespace DocumentGenerator.Data.Models.Data;

public class ColumnInfos
{
    /// <summary>
    /// Название колонки, которое находится в 1 строчке
    /// </summary>
    public string ColumnName { get; set; }
    
    /// <summary>
    /// Номер колонки (A1 => 1 / B1 => 2 / и тд)
    /// </summary>
    public string ColumnNumber { get; set; }
    
    /// <summary>
    /// Путь к таблице из которой нужно брать информацию о данной колонке
    /// </summary>
    public string SourceTable { get; set; }
}