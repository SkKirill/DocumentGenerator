using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DocumentGenerator.Data.Extensions;
using DocumentGenerator.Data.Models.Data;
using DocumentGenerator.Data.Models.DataBase.Output.NoTable;
using DocumentGenerator.Data.Services;
using DocumentGenerator.Data.Services.DataBase.Repositories;
using DocumentGenerator.Data.Services.Interfaces;
using DocumentGenerator.UI.Models.Pages;
using DocumentGenerator.UI.Services.Edit;
using DocumentGenerator.UI.Services.WindowsNavigation;
using DocumentGenerator.UI.ViewModels.ExtensionPages;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.Pages;

public class PageSettingsViewModel : ViewModelBase, IWindowNavigation, ISubscriber
{
    public IObservable<ViewTypes> RedirectToView => _redirectToView;
    private readonly Subject<ViewTypes> _redirectToView;
    public ObservableCollection<PageOrientations> Orientations { get; set; }
    public ObservableCollection<ExportFormats> Formats { get; set; }

    private PageOrientations _selectedOrientations;

    public PageOrientations SelectedOrientations
    {
        get => _selectedOrientations;
        set => this.RaiseAndSetIfChanged(ref _selectedOrientations, value);
    }

    private ExportFormats _selectedFormats;

    public ExportFormats SelectedFormats
    {
        get => _selectedFormats;
        set => this.RaiseAndSetIfChanged(ref _selectedFormats, value);
    }

    public DataFolderViewModel WatermarkFolder { get; set; }
    public DataFolderViewModel SaveToFolder { get; set; }

    public ReactiveCommand<Unit, Unit> GoBackCommand => _goBackCommand.Value;
    public ReactiveCommand<Unit, Unit> ContinueCommand => _continueCommand.Value;

    // Lazy команды
    private readonly Lazy<ReactiveCommand<Unit, Unit>> _goBackCommand;
    private readonly Lazy<ReactiveCommand<Unit, Unit>> _continueCommand;
    private readonly List<IDisposable> _subscriptions;
    private readonly IEnumerable<ILayoutNameNotifier> _layoutNameNotifiers;
    private readonly ILogger<PageSettingsViewModel> _logger;

    public PageSettingsViewModel(
        IEnumerable<ILayoutNameNotifier> layoutNameNotifiers,
        ILogger<PageSettingsViewModel> logger)
    {
        _subscriptions = new List<IDisposable>();
        _logger = logger;
        _redirectToView = new Subject<ViewTypes>();
        _layoutNameNotifiers = layoutNameNotifiers;

        Orientations = new ObservableCollection<PageOrientations>(
            (PageOrientations[])Enum.GetValues(typeof(PageOrientations))
            );
        Formats = new ObservableCollection<ExportFormats>((ExportFormats[])Enum.GetValues(typeof(ExportFormats)));

        _goBackCommand = new(() =>
            ReactiveCommand.CreateFromTask(RunGoBackAction, outputScheduler: RxApp.MainThreadScheduler));
        _continueCommand = new(() =>
            ReactiveCommand.CreateFromTask(RunContinue, outputScheduler: RxApp.MainThreadScheduler));

        SelectedOrientations = PageOrientations.Portrait;
        SelectedFormats = ExportFormats.Word;

        WatermarkFolder = new DataFolderViewModel();
        SaveToFolder = new DataFolderViewModel();
    }

    public void Subscribe()
    {
        foreach (var notifier in _layoutNameNotifiers)
        {
            _subscriptions.Add(notifier.NameEditLayout.Subscribe(LoadSetupForLayout));
        }
    }

    public void Unsubscribe()
    {
        _subscriptions.DisposeAndClear();
    }

    private void LoadSetupForLayout(string layoutName)
    {
        _logger.LogInformation($"Загрузка настроек макета: {layoutName}");
        using var layoutRepository = new LayoutRepository();
        layoutRepository.UseContext(new DatabaseContext());
        var layoutConfiguration = layoutRepository
            .GetLayoutsAsync().Result
            .First(item => item.Name.Equals(layoutName)).Configuration;

        if (layoutConfiguration is not ConfigurationModelPage config)
        {
            _logger.LogError($"Неверные настройки ожидался тип {typeof(ConfigurationModelPage)} {layoutConfiguration}");
            return;
        }

        WatermarkFolder.Folder = config.WatermarkFolder ?? string.Empty;
        SaveToFolder.Folder = config.SaveToFolder ?? string.Empty;
        SelectedFormats = config.ExportFormats;
        SelectedOrientations = config.PageOrientations;

        _logger.LogInformation("Загрузка настроек завершена...");
    }
    
    private async Task RunContinue()
    {
        // TODO: выполнить сохранение созданных макетов
        _redirectToView.OnNext(ViewTypes.Edit);
    }

    private async Task RunGoBackAction()
    {
        // TODO: не сохранять изменения
        _redirectToView.OnNext(ViewTypes.Edit);
    }
}