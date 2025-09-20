using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class AppData
{
    public List<VenueModel> Venues;
    public List<ReservationModel> Reservations;
    public Currency Currency;
    public int Notification;
    public float WasteBag;
    public float CO2E;
    public bool PermissionLocation;
    public bool PermissionNotification;
    public GeoPoint UserLocation;
    public List<NotificationModel> Notifications;
}
