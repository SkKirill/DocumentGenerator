using DocumentGenerator.UI;

namespace DocumentGenerator;

public class Program
{
    public static void Main(string[] args)
    {
        var provider = StartServices.ConfigureServices();
        
        UI.Program.Main(provider, args);
    }
}