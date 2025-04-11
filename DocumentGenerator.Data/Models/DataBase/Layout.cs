using DocumentGenerator.Data.Models.DataBase.Output;

namespace DocumentGenerator.Data.Models.DataBase;

public class Layout : BaseDatabaseModel
{
    public string Name { get; set; }
    
    public BaseConfigurationModel? Configuration { get; set; }
}