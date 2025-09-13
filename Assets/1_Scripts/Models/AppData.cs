using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class AppData
{
    public List<VenueModel> Venues;
    public List<ReservationModel> Reservations;

    public EventModel selectedEvent;
    public List<EventModel> events;
    public List<MapData> maps;
    public string name;
    public string phone;
    public string email;
    public bool notificationsSales = true;
    public bool notificationsEvents = true;
    public int Currency;
    
}
