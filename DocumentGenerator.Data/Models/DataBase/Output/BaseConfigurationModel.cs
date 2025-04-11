using DocumentGenerator.Data.Models.Data;

namespace DocumentGenerator.Data.Models.DataBase.Output;

public abstract class BaseConfigurationModel : BaseDatabaseModel
{
    public abstract ConfigurationTypes ConfigurationType { get; set; }
}