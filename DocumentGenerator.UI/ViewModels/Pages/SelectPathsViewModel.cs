using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Avalonia.Media;
using DocumentGenerator.UI.Models.ExtensionPages;
using DocumentGenerator.UI.Models.Pages;
using DocumentGenerator.UI.Services.WindowsNavigation;
using DocumentGenerator.UI.ViewModels.ExtensionPages;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.Pages;

public class SelectPathsViewModel : ViewModelBase, IManagerWindow
{
    public IObservable<ViewTypes> RedirectToView => _redirectToView;
    public ObservableCollection<DataPathItemViewModel> DataPaths { get; set; } = new();
    public DataFolderViewModel DataFolder { get; set; } = new();
    public ReactiveCommand<Unit, Unit> GoBackActionCommand => _goBackCommand.Value;
    public ReactiveCommand<Unit, Unit> ContinueActionCommand => _continueCommand.Value;

    private readonly Subject<ViewTypes> _redirectToView;
    private readonly ILogger<SelectPathsViewModel> _logger;

    // Lazy команды
    private readonly Lazy<ReactiveCommand<Unit, Unit>> _goBackCommand;
    private readonly Lazy<ReactiveCommand<Unit, Unit>> _continueCommand;

    public SelectPathsViewModel(ILogger<SelectPathsViewModel> logger)
    {
        _logger = logger;
        _redirectToView = new Subject<ViewTypes>();
        _goBackCommand = new(() =>
            ReactiveCommand.CreateFromTask(RunGoBackAction, outputScheduler: RxApp.MainThreadScheduler));
        _continueCommand = new(() =>
            ReactiveCommand.CreateFromTask(RunContinue, outputScheduler: RxApp.MainThreadScheduler));
        AddNewPathItem();
    }

    private List<string> GetValidPaths() =>
        DataPaths.Where(x => x.IsValid).Select(x => x.Path).ToList();

    private void AddNewPathItem()
    {
        var item = new DataPathItemViewModel(OnValidPathEntered);
        DataPaths.Add(item);
    }

    private void OnValidPathEntered(DataPathItemViewModel item)
    {
        if (DataPaths.Count(path => DataPaths.Last().Path == path.Path) > 1)
        {
            _logger.LogWarning($"Путь {item.Path} уже существует");
            item.HelpText = "Данный путь уже указан";
            item.HelpColor = Brushes.Orange;
            return;
        }

        // Добавить новый только если это последний элемент
        if (DataPaths.Last() == item && item.IsValid)
        {
            _logger.LogInformation($"Добавлен корректный путь к данным, путь = {item.Path}");
            AddNewPathItem();
        }
    }

    private async Task RunContinue()
    {
        if (GetValidPaths().Count <= 0)
        {
            DataPaths.Last().HelpText = "Требуется указать хотя бы 1 корректный путь к данным";
            DataPaths.Last().HelpColor = Brushes.Red;
            _logger.LogWarning($"Невозможен переход на следующую страницу, " +
                               $"путей к файлам с данными -> {GetValidPaths().Count}");
            return;
        }

        switch (DataFolder.IsValid)
        {
            case TypePathFolder.Invalid:
                DataFolder.HelpText = "Для продолжения нужно указать путь к папке с созданными файлами!";
                DataFolder.HelpColor = Brushes.Red;
                _logger.LogWarning("Папка не указана, переход на следующую страницу не будет осуществлен");
                return;
            case TypePathFolder.Valid:
                _logger.LogInformation($"Указана корректная папка, путь = {DataFolder.Folder}");
                break;
            case TypePathFolder.None:
                try
                {
                    Directory.CreateDirectory(DataFolder.Folder);
                    _logger.LogInformation($"Создана папка, путь = {DataFolder.Folder}");
                }
                catch (Exception exception)
                {
                    DataFolder.HelpText = "*не удалось создать папку..";
                    DataFolder.HelpColor = Brushes.Red;
                    _logger.LogWarning(exception, $"Не удалось создать папку {DataFolder.Folder}");
                    return;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(DataFolder.IsValid), DataFolder.IsValid, null);
        }

        _redirectToView.OnNext(ViewTypes.Layouts);
    }

    private async Task RunGoBackAction()
    {
        // Возвращает в текущую страницу (заглушка)
        _redirectToView.OnNext(ViewTypes.Path);
    }
}