namespace DocumentGenerator.Data.Models.DataUi;

public class InputData
{
    public List<string> ListLayouts { get; set; } = [];
    public List<string> DataPaths { get; set; } = [];
    public string DataFolder { get; set; } = string.Empty;
}