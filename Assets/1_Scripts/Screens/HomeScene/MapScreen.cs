using InfinityCode.OnlineMapsDemos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScreen : AppScreen
{
    [SerializeField] private OnlineMaps _map;
    [SerializeField] private OnlineMapsMarkerManager _markers;
    [SerializeField] private UIBubblePopup _bubbles;
    [SerializeField] private Canvas _canvasPopup;
    protected override void OnStart()
    {
        base.OnStart();
        InitPoints();
        _map.gameObject.SetActive(true);
        _canvasPopup.gameObject.SetActive(true);
        if (Data.PersonalManager.PermissionLocation) CreateUserPoint(Data.PersonalManager.UserPosition);
    }

    private void CreateUserPoint(GeoPoint point) 
    {
        _bubbles.CreatePoint(point);
    }

    public void GoToPoint(GeoPoint point)
    {
        _map.SetPosition(point.Longitude, point.Latitude);
        _map.zoom = 18;
    }

    private void InitPoints()
    {
        var allVenues = Data.VenueManager.GetVenuesWithCoordinates();
        _bubbles.venues = allVenues.ToArray();
    }

    private void OnDisable()
    {
        _map.gameObject.SetActive(false);
        _canvasPopup.gameObject.SetActive(false);
    }

}