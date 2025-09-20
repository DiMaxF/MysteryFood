using System;

public class PersonalManager : IDataManager
{
    private readonly AppData _appData;
    public bool PermissionNotification
    {
        get => _appData.PermissionNotification;
        set => _appData.PermissionNotification = value;
    }
    public bool PermissionLocation
    {
        get => _appData.PermissionLocation;
        set => _appData.PermissionLocation = value;
    }
    public Currency Currency 
    {
        get => _appData.Currency;
        set => _appData.Currency = value;
    }

    public GeoPoint UserPosition
    {
        get => _appData.UserLocation;
        set => _appData.UserLocation = value;
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
        Currency = Currency.EGP;
        Notification = 10;
        WasteBag = 0;
        CO2E = 0;  
    }

    public double CalculateDistance(GeoPoint point)
    {
        const double R = 6371.0;
        if (UserPosition == null) return 0;
        double lat1Rad = DegreesToRadians(UserPosition.Latitude);
        double lon1Rad = DegreesToRadians(UserPosition.Longitude);
        double lat2Rad = DegreesToRadians(point.Latitude);
        double lon2Rad = DegreesToRadians(point.Longitude);

        double deltaLat = lat2Rad - lat1Rad;
        double deltaLon = lon2Rad - lon1Rad;

        double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        double distance = R * c;
        return distance;
    }

    private double DegreesToRadians(float degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}