using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using DocumentGenerator.UI.Models;
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
    private Subject<bool> _completeView;

    public SelectLayoutsViewModel()
    {
        _completeView = new Subject<bool>();
        ClearActionButton = ReactiveCommand.Create(RunClearAction);
        ContinueButton = ReactiveCommand.Create(RunContinue);


        /* из бд достать что уже есть*/
        Items = new();
        Items.Add(new ListItemModel("Первый макет", () => Console.WriteLine("Изменить первый")));
        Items.Add(new ListItemModel("Второй макет", () => Console.WriteLine("Изменить второй")));
        Items.Add(new ListItemModel("Третий макет", () => Console.WriteLine("Изменить третий")));
        Items.Add(new ListItemModel("Четвертый макет", () => Console.WriteLine("Изменить третий")));
        Items.Add(new ListItemModel("Пятый макет", () => Console.WriteLine("Изменить третий")));

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
        // TODO: обработка нажатия в окошко поиска директории
    }

    public void EditItem(ListItemModel item)
    {
        item.EditAction();
    }

    public IEnumerable<ListItemModel> GetCheckedItems()
    {
        return Items.Where(item => item.IsChecked);
    }

    public void DoSomethingWithCheckedItems()
    {
        var checkedItems = GetCheckedItems();
        foreach (var item in checkedItems)
        {
            Console.WriteLine(item.Text);
        }
    }
}