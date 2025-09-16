using System;

public class PersonalManager : IDataManager
{
    private readonly AppData _appData;

    public Currency GetCurrency 
    {
        get => _appData.Currency;
        set => _appData.Currency = value;
    }
    public int Notification
    {
        get => _appData.Notification;
        set => _appData.Notification = value;
    }
    public float WasteBag
    {
        get => _appData.WasteBag;
        set => _appData.WasteBag = value;
    }
    public float CO2E
    {
        get => _appData.CO2E;
        set => _appData.CO2E = value;
    }
    public PersonalManager(AppData appData)
    {
        _appData = appData ?? throw new ArgumentNullException(nameof(appData));
    }

    public void Clear()
    {

    }
}