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

    public PersonalManager(AppData appData)
    {
        _appData = appData ?? throw new ArgumentNullException(nameof(appData));
    }

    public void Clear()
    {

    }
}