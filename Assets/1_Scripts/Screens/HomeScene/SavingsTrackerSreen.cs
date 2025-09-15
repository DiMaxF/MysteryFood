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
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
    }
}
