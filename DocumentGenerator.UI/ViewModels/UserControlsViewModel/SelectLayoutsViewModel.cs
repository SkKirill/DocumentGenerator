using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using DocumentGenerator.Data.Services;
using DocumentGenerator.Data.Services.DataBase.Repositories;
using DocumentGenerator.UI.Models;
using DocumentGenerator.UI.Services;
using DynamicData;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.UserControlsViewModel;

public class SelectLayoutsViewModel : ViewModelBase, IUserControlsNotifier
{
    public IObservable<UserControlTypes> RedirectToView => _redirectToView;

    public string NameEditLayout { get; set; }

    /// <summary>
    /// Список всех доступных к выбору макетов из базы данных
    /// </summary>
    public ObservableCollection<ListLayoutsModel> ListLayouts { get; set; }

    public ReactiveCommand<Unit, Unit> GoBackActionButton { get; }
    public ReactiveCommand<Unit, Unit> ContinueButton { get; }
    private readonly Subject<UserControlTypes> _redirectToView;
    private readonly List<IDisposable> _subscriptions;

    public SelectLayoutsViewModel()
    {
        _subscriptions = new List<IDisposable>();
        _redirectToView = new Subject<UserControlTypes>();
        GoBackActionButton = ReactiveCommand.Create(RunGoBackAction);
        ContinueButton = ReactiveCommand.Create(RunContinue);
        ListLayouts = [];

        using var layoutRepository = new LayoutRepository();
        layoutRepository.UseContext(new DatabaseContext());

        var layoutNames = layoutRepository.GetLayoutsAsync().Result
            .Select(item => item.Name).ToList();

        layoutNames.Add("Пустой макет");
        foreach (var name in layoutNames)
        {
            var layoutListModel = new ListLayoutsModel($"{name}");
            _subscriptions.Add(layoutListModel.NameEditLayout.Subscribe(EditItem));
            ListLayouts.Add(layoutListModel);
        }
    }

    private void EditItem(string name)
    {
        NameEditLayout = name;
        _redirectToView.OnNext(UserControlTypes.Edit);
    }

    private void RunContinue()
    {
        if (ListLayouts.Any(item => item.IsChecked))
        {
            _redirectToView.OnNext(UserControlTypes.Process);
            return;
        }
        // TODO: Сделать тостер, что пользователь не тыкнул ни в одну
    }

    private void RunGoBackAction()
    {
        _redirectToView.OnNext(UserControlTypes.Path);
    }

    public List<string> GetCheckedNames()
    {
        return ListLayouts
            .Where(item => item.IsChecked)
            .Select(item => item.NameLayout)
            .ToList();
    }
}