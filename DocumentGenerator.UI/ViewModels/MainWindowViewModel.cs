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
            { UserControlType.Path, new SelectPathsViewModel() }
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
        switch (ControlSelectPaths)
        {
            case SelectLayoutsViewModel:
            {
                ControlSelectPaths = _viewModels[UserControlType.Path];
                break;
            }
            case SelectPathsViewModel:
            {
                ControlSelectPaths = _viewModels[UserControlType.Layouts];
                break;
            }
        }
    }
}