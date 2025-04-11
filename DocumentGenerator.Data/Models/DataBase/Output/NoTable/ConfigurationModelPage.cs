using DocumentGenerator.Data.Models.Data;

namespace DocumentGenerator.Data.Models.DataBase.Output.NoTable;

public class ConfigurationModelPage : BaseConfigurationModel
{
    public override ConfigurationTypes ConfigurationType { get; set; } = ConfigurationTypes.Page;

    public PageOrientations PageOrientations { get; set; } = PageOrientations.Portrait;
    public ExportFormats ExportFormats { get; set; } = ExportFormats.Word;
    public string? SaveToFolder { get; set; }
    public string? WatermarkFolder { get; set; }
}