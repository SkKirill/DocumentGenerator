using DocumentGenerator.Data.Models.Data;

namespace DocumentGenerator.Core.Services.ReaderTables;

public interface IReadManager
{
    public List<ColumnInfos> CreateListColumns();
}