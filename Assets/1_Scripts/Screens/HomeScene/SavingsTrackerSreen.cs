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
    [SerializeField] private Text _totalSaved;
    [SerializeField] private Text _bagsCollected;
    [SerializeField] private Text _COeAvoided;
    [SerializeField] private Text _foodWastePrevented;
    [SerializeField] private RowChartView _monthlySavings;
    [SerializeField] private LineChartView _bagsOverTime;
    [SerializeField] private ListView _recentActivity;

    protected override void UpdateViews()
    {
        base.UpdateViews();
        _totalSaved.text = $"{Data.SavingsTrackerManager.GetTotalSaved()} {Data.PersonalManager.Currency}";
        _bagsCollected.text = $"{Data.SavingsTrackerManager.GetBagsCollected()}";
        _COeAvoided.text = $"{Data.SavingsTrackerManager.GetCO2EAvoided()} kg";
        _foodWastePrevented.text = $"{Data.SavingsTrackerManager.GetFoodWastePrevented()} kg";   
        UIContainer.InitView(_monthlySavings, Data.SavingsTrackerManager.GetMonthlySavingsChartData());
        UIContainer.InitView(_bagsOverTime, Data.SavingsTrackerManager.GetBagsOverTimeChartData());
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView(_back, (object _) => Container.Back().Forget());
        UIContainer.SubscribeToView(_orders, (object _) => Container.Show<OrdersScreen>());
    }
}
