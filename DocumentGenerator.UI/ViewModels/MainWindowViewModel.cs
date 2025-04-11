using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentGenerator.UI.Models.Extensions;
using DocumentGenerator.UI.Models.Pages;
using DocumentGenerator.UI.Services.WindowsNavigation;
using DocumentGenerator.UI.ViewModels.Pages;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase, IViewNavigation
{
    /// <summary>
    /// ViewModel, для навигации между экранами.
    /// </summary>
    public ViewModelBase CurrentView
    {
        get => _currentView;
        set
        {
            _currentView = value;
            this.RaisePropertyChanged();
        }
    }

    // Текущий отображаемый ViewModel
    private ViewModelBase _currentView;

    // Словарь для хранения всех возможных ViewModel по типу экрана
    private readonly Dictionary<ViewTypes, ViewModelBase> _viewModels;
    private readonly ILogger<MainWindowViewModel> _logger;

    public MainWindowViewModel(
        ILogger<MainWindowViewModel> logger,
        SelectLayoutsViewModel selectLayoutsViewModel,
        SelectPathsViewModel selectPathsViewModel,
        ProcessingViewModel processingViewModel,
        PageSettingsViewModel pageSettingsViewModel,
        EditLayoutViewModel editLayoutViewModel)
    {
        _logger = logger;
        // Инициализация словаря навигации
        _viewModels = new()
        {
            { ViewTypes.Layouts, selectLayoutsViewModel },
            { ViewTypes.Path, selectPathsViewModel },
            { ViewTypes.Edit, editLayoutViewModel },
            { ViewTypes.Process, processingViewModel },
            { ViewTypes.Settings, pageSettingsViewModel }
        };

        // Установка начального экрана
        CurrentView = _viewModels[ViewTypes.Path];
    }

    /// <summary>
    /// Метод навигации между экранами. Меняет текущий отображаемый ViewModel.
    /// </summary>
    public async Task<bool> OnRedirect(ViewTypes targetView)
    {
        if (!_viewModels.TryGetValue(targetView, out var newViewModel))
        {
            _logger.LogWarning($"Ошибка при перемещении на  страницу {targetView.ToDisplayString()}. Страница не найдена.");
            return false;
        }
        
        CurrentView = newViewModel;
        return true;
    }
}