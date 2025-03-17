using System;
using System.IO;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using DocumentGenerator.UI.Services;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.UserControlsViewModel;

public class SelectPathsViewModel : ViewModelBase, IUserControlsNotifier
{
    /// <summary>
    /// Событие по переходу на следующую или предыдущую форму
    /// </summary>
    public IObservable<UserControlTypes> RedirectToView => _redirectToView;

    /// <summary>
    /// Цвет строки подсказки под полем ввода пути к файлу с данными
    /// </summary>
    public IImmutableSolidColorBrush HelpLabelSearchPathColor
    {
        get => _helpLabelSearchPathColor;
        set => this.RaiseAndSetIfChanged(ref _helpLabelSearchPathColor, value);
    }
    
    public IImmutableSolidColorBrush HelpLabelSearchDictionaryPathColor
    {
        get => _helpLabelSearchDictionaryPathColor;
        set => this.RaiseAndSetIfChanged(ref _helpLabelSearchDictionaryPathColor, value);
    }

    /// <summary>
    /// Цвет строки подсказки под полем ввода папки для сохранения файлов
    /// </summary>
    public IImmutableSolidColorBrush HelpLabelSearchFolderColor
    {
        get => _helpLabelSearchFolderColor;
        set => this.RaiseAndSetIfChanged(ref _helpLabelSearchFolderColor, value);
    }

    /// <summary>
    /// Строка с подсказкой о существовании пути
    /// </summary>
    public string HelpLabelSearchPath
    {
        get => _helpLabelSearchPath;
        set => this.RaiseAndSetIfChanged(ref _helpLabelSearchPath, value);
    }
    public string HelpLabelSearchDictionaryPath
    {
        get => _helpLabelSearchDictionaryPath;
        set => this.RaiseAndSetIfChanged(ref _helpLabelSearchDictionaryPath, value);
    }

    /// <summary>
    /// Строка подсказки о существовании папки
    /// </summary>
    public string HelpLabelSearchFolder
    {
        get => _helpLabelSearchFolder;
        set => this.RaiseAndSetIfChanged(ref _helpLabelSearchFolder, value);
    }

    /// <summary>
    /// Строка с расположением файлов с исходными данными
    /// </summary>
    public string LocationDataText
    {
        get => _locationDataText;
        set
        {
            this.RaiseAndSetIfChanged(ref _locationDataText, value);
            ValidatePath(_locationDataText);
        }
    }
    
    public string LocationDictionaryDataText
    {
        get => _locationDictionaryDataText;
        set
        {
            this.RaiseAndSetIfChanged(ref _locationDictionaryDataText, value);
            ValidatePath(_locationDictionaryDataText);
        }
    }

    /// <summary>
    /// Строка с указанием папки для сохранения файлов
    /// </summary>
    public string LocationFolderSaveText
    {
        get => _locationFolderSaveText;
        set
        {
            this.RaiseAndSetIfChanged(ref _locationFolderSaveText, value);
            ValidateDirectory(_locationFolderSaveText);
        }
    }

    /// <summary>
    /// Команда при нажатии на кнопку поиска пути (лупа)
    /// </summary>
    public ReactiveCommand<Unit, Task> OpenSearchPathCommand { get; } 
    public ReactiveCommand<Unit, Task> OpenSearchDictionatyPathCommand { get; }

    /// <summary>
    /// Команда при нажатии на кнопку поиска папки (лупа)
    /// </summary>
    public ReactiveCommand<Unit, Task> OpenSearchFolderCommand { get; }

    /// <summary>
    /// Команда при нажатии на кнопку "Вернуться"
    /// </summary>
    public ReactiveCommand<Unit, Unit> GoBackActionCommand { get; }

    /// <summary>
    /// Команда при нажатии на кнопку "Продолжить"
    /// </summary>
    public ReactiveCommand<Unit, Unit> ContinueActionCommand { get; }

    private IImmutableSolidColorBrush _helpLabelSearchPathColor = null!;
    private IImmutableSolidColorBrush _helpLabelSearchDictionaryPathColor = null!;
    private IImmutableSolidColorBrush _helpLabelSearchFolderColor = null!;

    private string _helpLabelSearchPath = null!;
    private string _helpLabelSearchDictionaryPath = null!;
    private string _helpLabelSearchFolder = null!;

    private string _locationDataText = null!;
    private string _locationDictionaryDataText = null!;
    private string _locationFolderSaveText = null!;
    private readonly Subject<UserControlTypes> _redirectToView;

    [Obsolete("Obsolete")]
    public SelectPathsViewModel()
    {
        _redirectToView = new Subject<UserControlTypes>();
        LocationFolderSaveText = string.Empty;
        LocationDataText = string.Empty;
        LocationDictionaryDataText = string.Empty;

        OpenSearchPathCommand = ReactiveCommand.Create(RunSearchPath);
        OpenSearchDictionatyPathCommand = ReactiveCommand.Create(RunSearchDictionaryPath);
        OpenSearchFolderCommand = ReactiveCommand.Create(RunSearchFolder);
        GoBackActionCommand = ReactiveCommand.Create(RunGoBackAction);
        ContinueActionCommand = ReactiveCommand.Create(RunContinue);

        HelpLabelSearchPath = "*такого файла не существует";
        HelpLabelSearchDictionaryPath = "*такого файла не существует";
        HelpLabelSearchFolder = "*некорректно указана папка для создания";

        HelpLabelSearchFolderColor = Brushes.Red;
        HelpLabelSearchPathColor = Brushes.Red;
    }

    private void RunContinue()
    {
        var success = true;
        if (!ValidatePath(_locationDataText))
        {
            HelpLabelSearchPath = "*требуется указать путь к файлу с данными";
            HelpLabelSearchPathColor = Brushes.Red;
            success = false;
        }

        if (!ValidateDirectory(_locationFolderSaveText))
        {
            try
            {
                Directory.CreateDirectory(_locationFolderSaveText);
            }
            catch
            {
                HelpLabelSearchFolder = "*не удалось создать папку..";
                HelpLabelSearchFolderColor = Brushes.Red;
                success = false;
            }
        }

        if (success)
        {
            _redirectToView.OnNext(UserControlTypes.Layouts);
        }
    }

    /// <summary>
    /// Обработка нажатия кнопки вернуться (после чего произойдет возврат на прошлую форму)
    /// В случае с окном выбора путей эта кнопка недоступна и ничего не делает
    /// </summary>
    private void RunGoBackAction()
    {
        _redirectToView.OnNext(UserControlTypes.Path);
    }

    /// <summary>
    /// Обработка нажатия кнопки поиска папки
    /// </summary>
    [Obsolete("Obsolete")]
    private async Task RunSearchFolder()
    {
        var openFolderDialog = new OpenFolderDialog();

        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = desktop.MainWindow;
            if (window == null)
            {
                HelpLabelSearchFolder = "*не удалось открыть диалоговое окно";
                HelpLabelSearchFolderColor = Brushes.Red;
                return;
            }

            var result = await openFolderDialog.ShowAsync(window);
            if (!string.IsNullOrEmpty(result))
            {
                LocationFolderSaveText = result;
            }

            ValidateDirectory(LocationFolderSaveText);
        }
    }

    /// <summary>
    /// Обработка нажатия кнопки поиска пути к файлу
    /// </summary>
    [Obsolete("Obsolete")]
    private async Task RunSearchPath()
    {
        var openFileDialog = new OpenFileDialog()
        {
            AllowMultiple = false,
            Filters =
            [
                new FileDialogFilter
                {
                    Name = "Базы данных и Excel",
                    Extensions = { "xlsx", "xls", "ods", "db", "sqlite", "mdb", "accdb" }
                }
            ]
        };

        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = desktop.MainWindow;
            if (window == null)
            {
                HelpLabelSearchPath = "*не удалось открыть диалоговое окно";
                HelpLabelSearchPathColor = Brushes.Red;
                return;
            }

            var result = await openFileDialog.ShowAsync(window);
            if (result is { Length: > 0 })
            {
                LocationDataText = result[0];
            }

            ValidatePath(LocationDataText);
        }
    }
    private async Task RunSearchDictionaryPath()
    {
        var openFileDialog = new OpenFileDialog()
        {
            AllowMultiple = false,
            Filters =
            [
                new FileDialogFilter
                {
                    Name = "Базы данных и Excel",
                    Extensions = { "xlsx", "xls", "db", "ods", "sqlite", "mdb", "accdb" }
                }
            ]
        };

        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = desktop.MainWindow;
            if (window == null)
            {
                HelpLabelSearchPath = "*не удалось открыть диалоговое окно";
                HelpLabelSearchPathColor = Brushes.Red;
                return;
            }

            var result = await openFileDialog.ShowAsync(window);
            if (result is { Length: > 0 })
            {
                LocationDictionaryDataText = result[0];
            }

            ValidateDictionaryPath(LocationDictionaryDataText);
        }
    }

    /// <summary>
    /// Проверка на существование пути к файлу с данными
    /// </summary>
    /// <param name="path">Путь к файлу с данными</param>
    /// <returns>True -> если такой файл существует</returns>
    private bool ValidatePath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            HelpLabelSearchPath = "*путь не указан";
            HelpLabelSearchPathColor = Brushes.Red;
            return false;
        }

        if (File.Exists(path))
        {
            HelpLabelSearchPath = "*файл найден";
            HelpLabelSearchPathColor = Brushes.Green;
            return true;
        }

        HelpLabelSearchPath = "*неверно указан путь к файлу";
        HelpLabelSearchPathColor = Brushes.Red;
        return false;
    }
    private bool ValidateDictionaryPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            HelpLabelSearchDictionaryPath = "*путь не указан";
            HelpLabelSearchDictionaryPathColor = Brushes.Red;
            return false;
        }

        if (File.Exists(path))
        {
            HelpLabelSearchDictionaryPath = "*файл найден";
            HelpLabelSearchDictionaryPathColor = Brushes.Green;
            return true;
        }

        HelpLabelSearchDictionaryPath = "*неверно указан путь к файлу";
        HelpLabelSearchDictionaryPathColor = Brushes.Red;
        return false;
    }

    /// <summary>
    /// Проверка существования папки
    /// </summary>
    /// <param name="path">Путь к папке</param>
    /// <returns>True -> если папка существует</returns>
    private bool ValidateDirectory(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            HelpLabelSearchFolder = "*путь к папке не указан";
            HelpLabelSearchFolderColor = Brushes.Red;
            return false;
        }

        if (Directory.Exists(path))
        {
            HelpLabelSearchFolder = "*папка найдена";
            HelpLabelSearchFolderColor = Brushes.Green;
            return true;
        }

        HelpLabelSearchFolder = "*будет попытка создать данную папку";
        HelpLabelSearchFolderColor = Brushes.Orange;
        return false;
    }
}