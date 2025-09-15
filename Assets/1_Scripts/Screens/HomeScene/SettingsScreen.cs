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
    [Header("Overlay")] 
    [SerializeField] private ConfirmPanel _confirmPanel;

    private Currency _selectedCurrency => Data.PersonalManager.GetCurrency;
    private int _selectedNotification => Data.PersonalManager.Notification;


    protected override void UpdateViews()
    {
        base.UpdateViews();
        UpdateNotificationsToggles();
        UpdateCurrencyToggles();
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();

        UIContainer.SubscribeToView<ToggleView, bool>(_egp, val => SetCurrency(Currency.EGP));
        UIContainer.SubscribeToView<ToggleView, bool>(_eur, val => SetCurrency(Currency.EUR));
        UIContainer.SubscribeToView<ToggleView, bool>(_usd, val => SetCurrency(Currency.USD));

        UIContainer.SubscribeToView<ToggleView, bool>(_off, val => SetNotification(-1));
        UIContainer.SubscribeToView<ToggleView, bool>(_10min, val => SetNotification(10));
        UIContainer.SubscribeToView<ToggleView, bool>(_30min, val => SetNotification(30));
        UIContainer.SubscribeToView<ToggleView, bool>(_60min, val => SetNotification(60));

        UIContainer.SubscribeToView<InputTextView, string>(_CO2Bage, OnCOPerChange);
        UIContainer.SubscribeToView<InputTextView, string>(_wastePerBage, OnWasteBagChange);

        UIContainer.SubscribeToView(_testNotification, (object _) => OnButtonTestNotification());
        UIContainer.SubscribeToView(_openOS, (object _) => OnButtonOpenOSNotification());
        UIContainer.SubscribeToView(_restoreDefaults, (object _) => OnButtonRestoreDefaults());
        UIContainer.SubscribeToView(_clearAllData, (object _) => OnButtonClearData());
    }

    private void SetCurrency(Currency currency)
    {
        Data.PersonalManager.GetCurrency = currency;
        Data.SaveData();
        UpdateCurrencyToggles();
    }


    private void UpdateCurrencyToggles() 
    {
        UIContainer.InitView(_egp, _selectedCurrency == Currency.EGP);
        UIContainer.InitView(_eur, _selectedCurrency == Currency.EUR);
        UIContainer.InitView(_usd, _selectedCurrency == Currency.USD);

        Data.ReservationManager.UpdateCurrency(_selectedCurrency);
        Data.VenueManager.UpdateCurrency(_selectedCurrency);
        Data.SaveData();
    }

    private void SetNotification(int min)
    {
        Data.PersonalManager.Notification = min;
        Data.SaveData();
        UpdateNotificationsToggles();
    }

    private void UpdateNotificationsToggles()
    {
        UIContainer.InitView(_off, _selectedNotification == -1);
        UIContainer.InitView(_10min, _selectedNotification == 10);
        UIContainer.InitView(_30min, _selectedNotification == 30);
        UIContainer.InitView(_60min, _selectedNotification == 60);

    }

    private void OnWasteBagChange(string val) 
    {
    
    }

    private void OnCOPerChange(string val)
    {

    }                    



    private void OnButtonTestNotification() 
    {

    }

    private void OnButtonOpenOSNotification()
    {

    }

    private void OnButtonRestoreDefaults()
    {

    }

    private void OnButtonClearData()
    {

    }
}
