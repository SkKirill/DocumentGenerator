using DocumentGenerator.Data.Models.Data;

namespace DocumentGenerator.Data.Models.DataBase.Output.Table;

public class ConfigurationModelTable : BaseConfigurationModel
{
    public override ConfigurationTypes ConfigurationType { get; set; } = ConfigurationTypes.Table;
}