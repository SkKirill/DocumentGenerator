using System.Reactive.Subjects;
using Avalonia.Media;
using DocumentGenerator.Data.Extensions;
using DocumentGenerator.Data.Models;
using DocumentGenerator.Data.Models.Processing;
using DocumentGenerator.Data.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace DocumentGenerator.Core.Services;

public class StartProcess : IWriteProcessingNotifier, ISubscriber
{
    
    private Dictionary<string, string> _headers;
    public List<string> NameLayouts { get; set; }
    public List<string> SourceNames { get; set; }
    public string FolderTo { get; set; }




    public IStarterNotifier StarterNotifier { get; set; }
    public IObservable<ListItemProcessDto> WriteText => _writeText;
    
    private readonly Subject<ListItemProcessDto> _writeText;
    private readonly ILogger<StartProcess> _logger;
    private readonly List<IDisposable> _subscriptions;
    
    public StartProcess(ILogger<StartProcess> logger)
    {
        _subscriptions = new List<IDisposable>();
        _logger = logger;
        _writeText = new Subject<ListItemProcessDto>();
        
        
        /*_headers = new Dictionary<string, string>();
        SourceNames = new List<string>();
        FolderTo = string.Empty;
        NameLayouts = new List<string>();
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        foreach (var path in SourceNames)
        {
            using var package = new ExcelPackage(new FileInfo(path));

            foreach (var nameExcelWorksheet in package.Workbook.Worksheets)
            {
                foreach (var item in nameExcelWorksheet.Cells
                             .Where(item => item.ToString().Contains("1") && item.ToString().Length == 2))
                {
                    Console.WriteLine(item + " " + item.Value);
                    if (item.Value.ToString() != null)
                    {
                        _headers.Add(item.ToString(), item.Value.ToString()!);
                    }
                }
            }
        }

        Console.WriteLine("Starting document generator.");
        foreach (var item in NameLayouts)
        {
            Console.WriteLine(item);
        }*/
    }

    private void Starter(bool force)
    {
        if (force)
        {
            Start();
            return;
        }
        Finish();
    }

    private async Task Start()
    {
        _logger.LogInformation("Подготовка к созданию документов");
        _writeText.OnNext(new ListItemProcessDto(Brushes.MediumSeaGreen, "Подготовка приложения к созданию документов"));
        await Task.Delay(3000);
        _writeText.OnNext(new ListItemProcessDto(Brushes.MediumSeaGreen, "Подготовка прошла успешно!"));
    }

    private async Task Finish()
    {
        return;
    }

    public void ChangeNameLayout(List<string> name)
    {
        NameLayouts.Clear();
        NameLayouts.AddRange(name);
    }

    public void ChangeFolderTo(string folder)
    {
        FolderTo = folder;
    }

    public void Subscribe()
    {
        _subscriptions.Add(StarterNotifier.Start.Subscribe(Starter));
    }

    public void Unsubscribe()
    {
        _subscriptions.DisposeAndClear();
    }
}