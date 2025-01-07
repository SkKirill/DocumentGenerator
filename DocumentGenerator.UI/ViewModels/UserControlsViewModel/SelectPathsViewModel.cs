using System;
using System.Reactive;
using System.Reactive.Subjects;
using Avalonia.Media;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.UserControlsViewModel;

public class SelectPathsViewModel : ViewModelBase, IUserControlsNotifier
{
    private IImmutableSolidColorBrush _colorValidDataPath;

    public IImmutableSolidColorBrush ColorValidDataPath
    {
        get => _colorValidDataPath;
        set => this.RaiseAndSetIfChanged(ref _colorValidDataPath, value);
    }
    private IImmutableSolidColorBrush _colorValidSaveDataFolder;

    public IImmutableSolidColorBrush ColorValidSaveDataFolder
    {
        get => _colorValidSaveDataFolder;
        set => this.RaiseAndSetIfChanged(ref _colorValidSaveDataFolder, value);
    }
    
    public IObservable<bool> CompleteView => _completeView;
    public string ValidTextLabelDataPath { get; set; }
    public string ValidTextLabelFolderPath { get; set; }
    public ReactiveCommand<Unit, Unit> OpenSearchPath { get; }
    public ReactiveCommand<Unit, Unit> OpenSearchFolder { get; }
    public ReactiveCommand<Unit, Unit> ClearActionButton { get; }
    public ReactiveCommand<Unit, Unit> ContinueButton { get; }
    public string DataLocationTextBoxTextChanged
    {
        get => _dataLocationTextBoxTextChanged;
        set => _dataLocationTextBoxTextChanged = value;
    }
    public string SaveDataLocationTextBoxTextChanged
    {
        get => _saveFolderDataLocationTextBoxTextChanged;
        set => _saveFolderDataLocationTextBoxTextChanged = value;
    }
    
    private string _dataLocationTextBoxTextChanged;
    private string _saveFolderDataLocationTextBoxTextChanged;
    
    private Subject<bool> _completeView;
    public SelectPathsViewModel()
    {
        _completeView = new Subject<bool>();
        _saveFolderDataLocationTextBoxTextChanged = string.Empty;
        _dataLocationTextBoxTextChanged = string.Empty;
        
        ValidTextLabelDataPath = "*такого файла не существует";
        ValidTextLabelFolderPath = "*некорректно указана папка для создания";
        
        OpenSearchPath = ReactiveCommand.Create(RunSearchPath);
        OpenSearchFolder = ReactiveCommand.Create(RunSearchFolder);
        ClearActionButton = ReactiveCommand.Create(RunClearAction);
        ContinueButton = ReactiveCommand.Create(RunContinue);
        
        ColorValidSaveDataFolder = Brushes.Red;
        ColorValidDataPath = Brushes.Red;
    }

    private void RunContinue()
    {
        _completeView.OnNext(true);
    }

    private void RunClearAction()
    {
        _completeView.OnNext(false);
        // TODO: обработка нажатия в окошко поиска директории
    }

    private void RunSearchFolder()
    {
        // TODO: обработка нажатия в окошко поиска директории
    }
    private void RunSearchPath()
    {
        // TODO: обработка нажатия в окошко поиска пути
    }

}