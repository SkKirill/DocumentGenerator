using DocumentGenerator.Data.Models.Data;

namespace DocumentGenerator.Core.Services.ReaderTables.Readers;

public class NonReaderTable : IReaderTable
{
    public List<ColumnInfos> GetAllTablesInfos()
    {
        return [];
    }
}