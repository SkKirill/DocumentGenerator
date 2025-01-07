using System;
using System.Collections.Generic;
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

    private ViewModelBase _controlSelectPaths;
    private readonly List<IDisposable> _subscriptions;
    private Dictionary<UserControlType, ViewModelBase> _viewModels;

    public MainWindowViewModel()
    {
        _viewModels = new()
        {
            { UserControlType.Layouts, new SelectLayoutsViewModel() },
            { UserControlType.Path, new SelectPathsViewModel() },
            { UserControlType.Edit, new EditLayoutViewModel() },
            { UserControlType.Process, new ProcessingViewModel() }
        };
        ControlSelectPaths = _viewModels[UserControlType.Path];
        _subscriptions = new List<IDisposable>();
        foreach (var viewModel in _viewModels.Values)
        {
            if (viewModel is IUserControlsNotifier userControlsNotifier)
            {
                _subscriptions.Add(userControlsNotifier.CompleteView.Subscribe(OnComplete));
            }
        }
    }

    private void OnComplete(bool success)
    {
        if (success)
        {
            switch (ControlSelectPaths)
            {
                case SelectPathsViewModel:
                {
                    ControlSelectPaths = _viewModels[UserControlType.Layouts];
                    break;
                }
                case SelectLayoutsViewModel:
                {
                    ControlSelectPaths = _viewModels[UserControlType.Process];
                    break;
                }
                case EditLayoutViewModel:
                {
                    ControlSelectPaths = _viewModels[UserControlType.Layouts];
                    break;
                }
                case ProcessingViewModel:
                {
                    ControlSelectPaths = _viewModels[UserControlType.Edit];
                    break;
                }
            }
        }
        else
        {
            switch (ControlSelectPaths)
            {
                case SelectLayoutsViewModel:
                {
                    ControlSelectPaths = _viewModels[UserControlType.Path];
                    break;
                }
                case EditLayoutViewModel:
                {
                    ControlSelectPaths = _viewModels[UserControlType.Path];
                    break;
                }
                case ProcessingViewModel:
                {
                    ControlSelectPaths = _viewModels[UserControlType.Layouts];
                    break;
                }
            }
        }
    }
}