using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FilterOptions
{
    public DateTime? FromDate;
    public DateTime? ToDate;
    public int? VenueId;
    public List<StatusReservation> Statuses = new List<StatusReservation>();

    public float? MaxDistanceKm; 
    public float? MaxPrice; 
    public float? MinPrice; 
    public bool OnlyOpenNow;

}