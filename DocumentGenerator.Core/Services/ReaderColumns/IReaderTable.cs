using DocumentGenerator.Data.Models.Data;

namespace DocumentGenerator.Core.Services.ReaderColumns;

public interface IReaderTable
{
    public List<ColumnInfos> GetAllTablesInfos();
}