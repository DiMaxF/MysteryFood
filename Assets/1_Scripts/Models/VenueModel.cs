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
}
