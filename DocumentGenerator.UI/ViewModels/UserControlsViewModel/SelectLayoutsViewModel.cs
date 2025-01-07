using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.UserControlsViewModel;

public class SelectLayoutsViewModel : ViewModelBase, IUserControlsNotifier
{
    public ObservableCollection<ListItemViewModel> Items { get; set; }
    public ReactiveCommand<ListItemViewModel, Unit> EditItemCommand { get; set; }
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
        Items.Add(new ListItemViewModel("Первый макет", () => Console.WriteLine("Изменить первый")));
        Items.Add(new ListItemViewModel("Второй макет", () => Console.WriteLine("Изменить второй")));
        Items.Add(new ListItemViewModel("Третий макет", () => Console.WriteLine("Изменить третий")));
        Items.Add(new ListItemViewModel("Четвертый макет", () => Console.WriteLine("Изменить третий")));
        Items.Add(new ListItemViewModel("Пятый макет", () => Console.WriteLine("Изменить третий")));

        EditItemCommand = ReactiveCommand.Create<ListItemViewModel>(EditItem);
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

    public void EditItem(ListItemViewModel item)
    {
        item.EditAction();
    }

    public IEnumerable<ListItemViewModel> GetCheckedItems()
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