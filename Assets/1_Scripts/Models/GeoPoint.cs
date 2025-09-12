using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GeoPoint
{
    public string Address;
    public float Latitude;  
    public float Longitude;
    public GeoPoint(float latitude, float longitude, string address)
    {
        Latitude = latitude;
        Longitude = longitude;
        Address = address;
    }
    public GeoPoint(Vector2 coordinates, string address)
    {
        Longitude = coordinates.x;
        Latitude = coordinates.y;
        Address = address;
    }

    public Vector2 ToVector2()
    {
        return new Vector2(Longitude, Latitude);
    }

    public OnlineMapsVector2d ToOnlineMapsVector2d()
    {
        return new OnlineMapsVector2d(Longitude, Latitude);
    }
}
