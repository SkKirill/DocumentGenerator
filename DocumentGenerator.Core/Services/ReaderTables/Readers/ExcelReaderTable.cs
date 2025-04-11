using DocumentGenerator.Data.Models.Data;
using Excel = Microsoft.Office.Interop.Excel;

namespace DocumentGenerator.Core.Services.ReaderTables.Readers;

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

        var excelApp = new Excel.Application();
        Excel.Workbook excelWorkbook = null!;

        try
        {
            excelWorkbook = excelApp.Workbooks.Open(_excelFilePath);

            for (var countSheet = 1; countSheet <= excelWorkbook.Sheets.Count; countSheet++)
            {
                var excelSheet = (Excel.Worksheet)excelWorkbook.Sheets[countSheet];
                var usedRange = excelSheet.UsedRange;
                var columnCount = usedRange.Columns.Count;

                for (var numColumn = 1; numColumn <= columnCount; numColumn++)
                {
                    var cellValue = (usedRange.Cells[1, numColumn] as Excel.Range)?.Text;
                    if (cellValue == null)
                        continue;

                    columnInfos.Add(new ColumnInfos
                    {
                        ColumnName = cellValue.ToString(),
                        ColumnNumber = numColumn,
                        SourceTable = _excelFilePath
                    });
                }
            }
        }
        finally
        {
            // Очистка ресурсов
            excelWorkbook?.Close(false);
            excelApp.Quit();
            
#pragma warning disable CA1416
            if (excelWorkbook != null)
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
#pragma warning restore CA1416
        }

        return columnInfos;
    }
}