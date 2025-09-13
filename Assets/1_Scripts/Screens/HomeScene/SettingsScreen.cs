using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsScreen : AppScreen
{
    [Header("Currency")]
    [SerializeField] private ToggleView _egp;
    [SerializeField] private ToggleView _eur;
    [SerializeField] private ToggleView _usd;

    [Header("Notification")]
    [SerializeField] private ToggleView _off;
    [SerializeField] private ToggleView _10min;
    [SerializeField] private ToggleView _30min;
    [SerializeField] private ToggleView _60min;
    [SerializeField] private ButtonView _testNotification;
    [SerializeField] private ButtonView _openOS;

    [Header("Environmental Factors")]
    [SerializeField] private InputTextView _wastePerBage;
    [SerializeField] private InputTextView _CO2Bage;
    [SerializeField] private ButtonView _restoreDefaults;

    [Header("Data")]
    [SerializeField] private ButtonView _clearAllData;


    protected override void UpdateViews()
    {
        base.UpdateViews();
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
    }

    private void UpdateCurrencyToggles() 
    {
    
    }

    private void UpdateNotificationsToggles()
    {

    }
}
