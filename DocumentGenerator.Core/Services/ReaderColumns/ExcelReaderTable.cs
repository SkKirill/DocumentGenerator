using DocumentGenerator.Data.Models.Data;
using Excel = Microsoft.Office.Interop.Excel;

namespace DocumentGenerator.Core.Services.ReaderColumns;

public class ExcelReaderTable : IReaderTable
{
    private readonly string _excelFilePath;

    public ExcelReaderTable(string excelFilePath)
    {
        _excelFilePath = excelFilePath;
    }

    public List<ColumnInfos> GetAllTablesInfos()
    {
        var columnInfos = new List<ColumnInfos>();
        var worckShets = new Excel.Application();

        return columnInfos;
    }
}