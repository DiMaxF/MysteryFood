using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenueManager : IDataManager
{
    private readonly AppData _appData;

    public VenueManager(AppData appData)
    {
        _appData = appData ?? throw new ArgumentNullException(nameof(appData));
    }

    public void Clear()
    {

    }

    public void AddVenue(VenueModel venue) 
    {
        _appData.Venues.Add(venue); 
    }
}
