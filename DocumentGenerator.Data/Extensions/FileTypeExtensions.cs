using DocumentGenerator.Data.Models.Data;

namespace DocumentGenerator.Data.Extensions;

public static class FileTypeExtensions
{
    private static readonly Dictionary<string, TableFileTypes> ExtensionMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { ".xlsx", TableFileTypes.Xlsx },
            { ".xls", TableFileTypes.Xls },
            { ".xlsb", TableFileTypes.Xlsb },
            { ".ods", TableFileTypes.Ods },
            { ".csv", TableFileTypes.Csv },
            { ".tsv", TableFileTypes.Tsv },
            { ".json", TableFileTypes.Json },
            { ".xml", TableFileTypes.Xml },
            { ".parquet", TableFileTypes.Parquet },
            { ".feather", TableFileTypes.Feather },
            { ".db", TableFileTypes.Db },
            { ".sqlite", TableFileTypes.Sqlite },
            { ".accdb", TableFileTypes.Accdb },
            { ".sav", TableFileTypes.Sav },
            { ".dta", TableFileTypes.Dta },
            { ".dbf", TableFileTypes.Dbf },
            { ".arff", TableFileTypes.Arff },
            { ".h5", TableFileTypes.H5 },
            { ".hdf5", TableFileTypes.Hdf5 },
            { ".numbers", TableFileTypes.Numbers }
        };

    public static TableFileTypes GetTableFileTypes(this string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return TableFileTypes.Unknown;
        }

        var ext = Path.GetExtension(filePath)?.ToLower();
        if (ext != null && ExtensionMap.TryGetValue(ext, out var type))
        {
            return type;
        }

        return TableFileTypes.Unknown;
    }
}