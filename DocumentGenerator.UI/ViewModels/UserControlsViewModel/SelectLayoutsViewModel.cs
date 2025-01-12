using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using DocumentGenerator.UI.Models;
using DocumentGenerator.UI.Services;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.UserControlsViewModel;

public class SelectLayoutsViewModel : ViewModelBase, IUserControlsNotifier
{
    public ObservableCollection<ListItemModel> Items { get; set; }
    public ReactiveCommand<ListItemModel, Unit> EditItemCommand { get; set; }
    public ReactiveCommand<Unit, Unit> DoSomethingCommand { get; set; }
    public IObservable<bool> CompleteView => _completeView;
    public ReactiveCommand<Unit, Unit> ClearActionButton { get; }
    public ReactiveCommand<Unit, Unit> ContinueButton { get; }
    private readonly Subject<bool> _completeView;

    public SelectLayoutsViewModel()
    {
        _completeView = new Subject<bool>();
        ClearActionButton = ReactiveCommand.Create(RunClearAction);
        ContinueButton = ReactiveCommand.Create(RunContinue);


        /* из бд достать что уже есть*/
        Items =
        [
            new ListItemModel("Первый макет", () => Console.WriteLine("Изменить первый"))
        ];

        EditItemCommand = ReactiveCommand.Create<ListItemModel>(EditItem);
        DoSomethingCommand = ReactiveCommand.Create(DoSomethingWithCheckedItems);
    }

    private void RunContinue()
    {
        _completeView.OnNext(true);
    }

    private void RunClearAction()
    {
        _completeView.OnNext(false);
    }

    private static void EditItem(ListItemModel item)
    {
        item.EditAction();
    }

    private IEnumerable<ListItemModel> GetCheckedItems()
    {
        return Items.Where(item => item.IsChecked);
    }

    private void DoSomethingWithCheckedItems()
    {
        var checkedItems = GetCheckedItems();
        foreach (var item in checkedItems)
        {
            Console.WriteLine(item.Text);
        }
    }
}