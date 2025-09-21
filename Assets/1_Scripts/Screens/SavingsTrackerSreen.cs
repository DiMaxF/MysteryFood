using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavingsTrackerSreen : AppScreen
{
    [SerializeField] private ButtonView _filters;
    [SerializeField] private ButtonView _back;
    [SerializeField] private ButtonView _orders;
    [SerializeField] private ButtonView _settingsButton;
    [SerializeField] private Text _totalSaved;
    [SerializeField] private Text _bagsCollected;
    [SerializeField] private Text _COeAvoided;
    [SerializeField] private Text _foodWastePrevented;
    [SerializeField] private RowChartView _monthlySavings;
    [SerializeField] private LineChartView _bagsOverTime;
    [SerializeField] private ListView _recentActivity;
    [SerializeField] private BaseView _noOrders;
    [SerializeField] private ButtonView _goOrders;
    [Header("Overlay")]
    [SerializeField] private FiltersSavingsView _filtersView;
    private FilterOptions _filtersOptions = new FilterOptions();

    private List<TableElement.Data> _table = new List<TableElement.Data>();
    protected override void OnStart()
    {
        _filtersOptions.Statuses = new List<StatusReservation> { StatusReservation.PickedUp };
        Data.SavingsTrackerManager.ApplyFilters(_filtersOptions);

        base.OnStart();
        UIContainer.RegisterView(_filtersView);
        UIContainer.InitView(_filtersView, _filtersOptions);
        _filtersView.Hide();

        
    }

    private List<TableElement.Data> GenerateTableData(List<ReservationModel> list) 
    {
        var l = new List<TableElement.Data>();
        foreach (var a in list) 
        {
            var v = Data.VenueManager.GetById(a.VenueId);
            l.Add(new TableElement.Data(a.CreatedAt, v.Name, a.Quantity.ToString(), $"{a.OriginalPrice.Amount - a.DiscountedPrice.Amount} {a.DiscountedPrice.Currency}"));
        }
        return l;
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        _totalSaved.text = $"{Data.SavingsTrackerManager.GetTotalSaved()} {Data.PersonalManager.Currency}";
        _bagsCollected.text = $"{Data.SavingsTrackerManager.GetBagsCollected()}";
        _COeAvoided.text = $"{Data.SavingsTrackerManager.GetCO2EAvoided()} kg";
        _foodWastePrevented.text = $"{Data.SavingsTrackerManager.GetFoodWastePrevented()} kg";   
        UIContainer.InitView(_monthlySavings, Data.SavingsTrackerManager.GetMonthlySavingsChartData());
        UIContainer.InitView(_bagsOverTime, Data.SavingsTrackerManager.GetBagsOverTimeChartData());
        _table = new List<TableElement.Data>();
        _table.Add(new TableElement.Data("Date", "Venue", "Qty", "Saved per order"));
        _table.AddRange(GenerateTableData(Data.SavingsTrackerManager.GetRecentActivity(5)));
        UIContainer.InitView(_recentActivity, _table);
        if (Data.SavingsTrackerManager.GetFoodWastePrevented() != 0 || Data.SavingsTrackerManager.GetCO2EAvoided() != 0) _settingsButton.Hide();
        else _settingsButton.Show();


        if (Data.SavingsTrackerManager.GetFilteredReservations().Count == 0) _noOrders.Show();
        else _noOrders.Hide();
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView(_back, (object _) => Container.Back().Forget());
        UIContainer.SubscribeToView(_goOrders, (object _) => Container.Show<OrdersScreen>());
        UIContainer.SubscribeToView(_orders, (object _) => Container.Show<OrdersScreen>());
        UIContainer.SubscribeToView(_filters, (object _) => ShowFilters());
        UIContainer.SubscribeToView(_settingsButton, (object _) => Container.Show<SettingsScreen>());
        UIContainer.SubscribeToView<FiltersSavingsView, FilterOptions>(_filtersView, ApplyFilters);
    }
    private void ApplyFilters(FilterOptions filters) 
    {
        Data.SavingsTrackerManager.ApplyFilters(filters);
        UpdateViews();
        _filtersView.Hide();
    }

    private void ShowFilters() 
    {
        _filtersView.Show();
    }
}
