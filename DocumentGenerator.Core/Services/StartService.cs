namespace DocumentGenerator.Core.Services;

public class StartService
{
    public List<string> NameLayouts { get; set; }

    public StartService(List<string> nameLayouts)
    {
        NameLayouts = nameLayouts;
        Console.WriteLine("Starting document generator.");
        foreach (var item in NameLayouts)
        {
            Console.WriteLine(item);
        }
    }
}