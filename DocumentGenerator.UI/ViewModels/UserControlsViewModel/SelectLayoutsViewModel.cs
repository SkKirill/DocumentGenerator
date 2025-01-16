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
using ReactiveUI;

namespace DocumentGenerator.UI.ViewModels.UserControlsViewModel;

public class SelectLayoutsViewModel : ViewModelBase, IUserControlsNotifier
{
    public ObservableCollection<ListLayoutsModel> Layouts { get; set; }
    public ReactiveCommand<ListLayoutsModel, Unit> EditItemCommand { get; set; }
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
        Layouts = new();

        using var layoutRepository = new LayoutRepository();
        layoutRepository.UseContext(new DatabaseContext());

        var layouts = layoutRepository.GetLayoutsAsync().Result;

        foreach (var layout in layouts)
        {
            Layouts.Add(new ListLayoutsModel($"{layout.Name}", () => Console.WriteLine(layout.Name)));
        }

        EditItemCommand = ReactiveCommand.Create<ListLayoutsModel>(EditItem);
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

    private static void EditItem(ListLayoutsModel layouts)
    {
        layouts.EditAction();
    }

    private IEnumerable<ListLayoutsModel> GetCheckedItems()
    {
        return Layouts.Where(item => item.IsChecked);
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