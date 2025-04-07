using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using DocumentGenerator.UI.Models.ExtensionPages;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.ExtensionPages;

public class DataFolderViewModel : ViewModelBase
{
    public string Folder
    {
        get => _folder;
        set
        {
            this.RaiseAndSetIfChanged(ref _folder, value);
            ValidateDirectory();
        }
    }

    public IImmutableSolidColorBrush HelpColor
    {
        get => _helpColor;
        set => this.RaiseAndSetIfChanged(ref _helpColor, value);
    }

    public string HelpText
    {
        get => _helpText;
        set => this.RaiseAndSetIfChanged(ref _helpText, value);
    }

    public TypePathFolder IsValid { get; private set; }
    public ReactiveCommand<Unit, Unit> BrowseFolderCommand => _browseFolderCommand.Value;

    private string _folder;
    private IImmutableSolidColorBrush _helpColor;
    private string _helpText;
    private readonly Lazy<ReactiveCommand<Unit, Unit>> _browseFolderCommand;

    public DataFolderViewModel()
    {
        IsValid = TypePathFolder.Invalid;
        _helpText = "*некорректно указана папка для создания";
        _folder = string.Empty;
        _helpColor = Brushes.Red;
        _browseFolderCommand = new(() =>
            ReactiveCommand.CreateFromTask(OpenFolderDialog, outputScheduler: RxApp.MainThreadScheduler));
    }

    private async Task OpenFolderDialog()
    {
        var openFolderDialog = new OpenFolderDialog();
        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = desktop.MainWindow;
            if (window == null)
            {
                HelpText = "*не удалось открыть диалоговое окно";
                HelpColor = Brushes.Red;
                return;
            }

            var result = await openFolderDialog.ShowAsync(window);
            if (!string.IsNullOrEmpty(result))
            {
                Folder = result;
            }

            ValidateDirectory();
        }
    }

    private void ValidateDirectory()
    {
        if (string.IsNullOrEmpty(Folder))
        {
            HelpText = "*путь к папке не указан";
            HelpColor = Brushes.Red;
            IsValid = TypePathFolder.Invalid;
            return;
        }

        if (Directory.Exists(Folder))
        {
            HelpText = "*папка найдена";
            HelpColor = Brushes.Green;
            IsValid = TypePathFolder.Valid;
            return;
        }

        HelpText = "*будет попытка создать данную папку";
        HelpColor = Brushes.Orange;
        IsValid = TypePathFolder.None;
    }
}