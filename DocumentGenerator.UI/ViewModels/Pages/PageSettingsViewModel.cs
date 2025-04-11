using System;
using System.Collections.ObjectModel;
using DocumentGenerator.Data.Models.Data;
using DocumentGenerator.UI.ViewModels.ExtensionPages;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.Pages;

public class PageSettingsViewModel : ViewModelBase
{
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

    public PageSettingsViewModel()
    {
        Orientations = new ObservableCollection<PageOrientations>((PageOrientations[])Enum.GetValues(typeof(PageOrientations)));
        Formats = new ObservableCollection<ExportFormats>((ExportFormats[])Enum.GetValues(typeof(ExportFormats)));

        SelectedOrientations = PageOrientations.Portrait;
        SelectedFormats = ExportFormats.Pdf;

        WatermarkFolder = new DataFolderViewModel();
        SaveToFolder = new DataFolderViewModel();
    }
}