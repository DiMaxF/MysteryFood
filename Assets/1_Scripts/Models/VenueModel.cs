using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VenueModel 
{
    public string Name;
    public GeoPoint Location;
    public int Phone;
    public CurrencyModel Price;
    public string StartTime;
    public string EndTime;
    public string Description;
    public string IngredientsAllergens;
    public string ImagePath;

    public VenueModel()
    {
        Name = string.Empty;
        Location = new GeoPoint(Vector2.zero, "");
        Phone = 0;
        Price = new CurrencyModel();
        StartTime = string.Empty;
        EndTime = string.Empty;
        Description = string.Empty;
        IngredientsAllergens = string.Empty;
        ImagePath = string.Empty;
    }
}
