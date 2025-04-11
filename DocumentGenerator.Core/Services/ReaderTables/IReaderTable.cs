using DocumentGenerator.Data.Models.Data;

namespace DocumentGenerator.Core.Services.ReaderTables;

public interface IReaderTable
{
    public List<ColumnInfos> GetAllTablesInfos();
}