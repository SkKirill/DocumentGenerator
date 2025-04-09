using System.Reactive.Subjects;
using Avalonia.Media;
using DocumentGenerator.Data.Extensions;
using DocumentGenerator.Data.Models.DataUi;
using DocumentGenerator.Data.Models.Processing;
using DocumentGenerator.Data.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace DocumentGenerator.Core.Services;

public class StartProcess : IWriteProcessingNotifier, ISubscriber
{
    public IStarterNotifier StarterNotifier { get; set; }
    public IObservable<ListItemProcessDto> WriteText => _writeText;
    
    private readonly Subject<ListItemProcessDto> _writeText;
    private readonly ILogger<StartProcess> _logger;
    private readonly List<IDisposable> _subscriptions;
    private readonly InputData _inputData;
    public StartProcess(
        ILogger<StartProcess> logger,
        InputData inputData)
    {
        _inputData = inputData;
        _subscriptions = new List<IDisposable>();
        _logger = logger;
        _writeText = new Subject<ListItemProcessDto>();
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