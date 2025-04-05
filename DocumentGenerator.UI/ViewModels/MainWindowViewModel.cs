using System;
using System.Collections.Generic;
using System.Linq;
using DocumentGenerator.Core.Services;
using DocumentGenerator.UI.Services;
using DocumentGenerator.UI.ViewModels.UserControlsViewModel;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ViewModelBase ControlSelectPaths
    {
        get => _controlSelectPaths;
        set
        {
            _controlSelectPaths = value;
            this.RaisePropertyChanged();
        }
    }

    private StartService _startService;
    private ViewModelBase _controlSelectPaths;
    private readonly List<IDisposable> _subscriptions;
    private readonly Dictionary<UserControlTypes, ViewModelBase> _viewModels;
    
    public MainWindowViewModel()
    {
        _subscriptions = new List<IDisposable>();
        _viewModels = new()
        {
            { UserControlTypes.Layouts, new SelectLayoutsViewModel() },
            { UserControlTypes.Path, new SelectPathsViewModel() },
            { UserControlTypes.Process, new ProcessingViewModel() }
        };
        ControlSelectPaths = _viewModels[UserControlTypes.Path];
        foreach (var viewModel in _viewModels.Values)
        {
            if (viewModel is IUserControlsNotifier userControlsNotifier)
            {
                _subscriptions.Add(userControlsNotifier.RedirectToView.Subscribe(OnComplete));
            }
        }
    }

    private void OnComplete(UserControlTypes success)
    {
        if (_viewModels[UserControlTypes.Layouts] is SelectLayoutsViewModel selectLayoutsViewModel)
        {
            switch (success)
            {
                case UserControlTypes.Edit:
                    var editLayoutViewModel = new EditLayoutViewModel(selectLayoutsViewModel.NameEditLayout);
                    if (editLayoutViewModel is IUserControlsNotifier userControlsNotifier)
                    {
                        _subscriptions.Add(userControlsNotifier.RedirectToView.Subscribe(OnComplete));
                    }

                    _viewModels[UserControlTypes.Edit] = editLayoutViewModel;
                    break;

                case UserControlTypes.Process:
                    var viewPath = _viewModels[UserControlTypes.Path] as SelectPathsViewModel;
                    _startService = new StartService(selectLayoutsViewModel.GetCheckedNames(), 
                        [viewPath.LocationDataText], viewPath.LocationFolderSaveText);
                    break;
            }
        }

        ControlSelectPaths = _viewModels[success];
    }
}