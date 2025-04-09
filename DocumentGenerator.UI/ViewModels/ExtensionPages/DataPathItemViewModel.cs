using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.ExtensionPages;

public class DataPathItemViewModel : ViewModelBase
{
    public string Path
    {
        get => _path;
        set
        {
            this.RaiseAndSetIfChanged(ref _path, value);
            ValidatePath();
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

    public bool IsValid { get; private set; }
    public ReactiveCommand<Unit, Unit> BrowseCommand => _browseCommand.Value;

    private readonly Action<DataPathItemViewModel> _onValidPath;
    private string _path = string.Empty;
    private IImmutableSolidColorBrush _helpColor = Brushes.Red;
    private string _helpText = "*некорректно указан путь для создания";
    private readonly Lazy<ReactiveCommand<Unit, Unit>> _browseCommand;

    public DataPathItemViewModel(Action<DataPathItemViewModel> onValidPath)
    {
        _onValidPath = onValidPath;
        _browseCommand = new(() =>
            ReactiveCommand.CreateFromTask(OpenFileDialog, outputScheduler: RxApp.MainThreadScheduler));
    }

    private async Task OpenFileDialog()
    {
        var openFileDialog = new OpenFileDialog()
        {
            AllowMultiple = false,
            Filters =
            [
                new FileDialogFilter
                {
                    Name = "Базы данных и Excel",
                    Extensions = { "xlsx", "xls", "db", "sqlite", "mdb", "accdb", "ods" }
                }
            ]
        };

        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = desktop.MainWindow;
            if (window == null)
            {
                HelpText = "*не удалось открыть диалоговое окно";
                HelpColor = Brushes.Red;
                return;
            }

            var result = await openFileDialog.ShowAsync(window);
            if (result is { Length: > 0 })
            {
                Path = result[0];
            }

            ValidatePath();
        }
    }

    private void ValidatePath()
    {
        if (string.IsNullOrWhiteSpace(Path))
        {
            HelpText = "*путь не указан";
            HelpColor = Brushes.Red;
            IsValid = false;
            return;
        }

        if (File.Exists(Path))
        {
            HelpText = "*файл найден";
            HelpColor = Brushes.Green;
            IsValid = true;
            _onValidPath(this);
        }
        else
        {
            HelpText = "*неверно указан путь к файлу";
            HelpColor = Brushes.Red;
            IsValid = false;
        }
    }
}