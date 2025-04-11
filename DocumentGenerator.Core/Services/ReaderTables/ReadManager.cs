using DocumentGenerator.Core.Services.ReaderTables;
using DocumentGenerator.Core.Services.ReaderTables.Readers;
using DocumentGenerator.Data.Extensions;
using DocumentGenerator.Data.Models.Data;
using DocumentGenerator.Data.Models.DataUi;
using Microsoft.Extensions.Logging;

namespace DocumentGenerator.Core.Services.ReaderTables;

public class ReadManager : IReadManager
{
    private readonly ILogger<ReadManager> _logger;
    private readonly InputData _inputData;
    
    public ReadManager(
        ILogger<ReadManager> logger,
        InputData inputData)
    {
        _inputData = inputData;
        _logger = logger;
    }

    public List<ColumnInfos> CreateListColumns()
    {
        try
        {
            _logger.LogInformation("Чтение всех атрибутов таблиц...");
            if (_inputData.DataPaths.Count == 0)
            {
                _logger.LogWarning("Не задан не один путь к данным!");
            }

            var columns = new List<ColumnInfos>();
            IReaderTable reader;

            foreach (var path in _inputData.DataPaths)
            {
                switch (path.GetTableFileTypes())
                {
                    case TableFileTypes.Xlsx:
                    case TableFileTypes.Xls:
                    case TableFileTypes.Ods:
                        reader = new ExcelReaderTable(path);
                        break;

                    case TableFileTypes.Db:
                    case TableFileTypes.Sqlite:
                    case TableFileTypes.Accdb:
                        reader = new NonReaderTable();
                        _logger.LogWarning(
                            $"Временно не доступны реализации чтения файла {Enum.GetName(path.GetTableFileTypes())}");
                        break;

                    case TableFileTypes.Unknown:
                    case TableFileTypes.Csv:
                    case TableFileTypes.Xlsb:
                    case TableFileTypes.Tsv:
                    case TableFileTypes.Json:
                    case TableFileTypes.Xml:
                    case TableFileTypes.Parquet:
                    case TableFileTypes.Feather:
                    case TableFileTypes.Sav:
                    case TableFileTypes.Dta:
                    case TableFileTypes.Dbf:
                    case TableFileTypes.Arff:
                    case TableFileTypes.H5:
                    case TableFileTypes.Hdf5:
                    case TableFileTypes.Numbers:
                        reader = new NonReaderTable();
                        _logger.LogWarning(
                            $"Запланирована реализация обработки файлов {Enum.GetName(path.GetTableFileTypes())} в будущем");
                        break;
                    default:
                        _logger.LogError($"Указан неверный тип файла с данными {Path.GetExtension(path)?.ToLower()}");
                        throw new ArgumentOutOfRangeException();
                }
                
                var columnInfos = reader.GetAllTablesInfos();
                _logger.LogInformation($"Файл: {path}, колонок {columnInfos.Count}, " +
                                       $"названия: {string.Join(',', columnInfos.Select(item => item.ColumnName))}");
                
                columns.AddRange(columnInfos);
            }

            _logger.LogInformation($"Чтение файлов завершено, файлов: {_inputData.DataPaths.Count}");
            return columns;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Возникла ошибка при попытке чтения таблиц с данными");
            return [];
        }
    }
}