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
    }

    private void InitPoints() 
    {
        var allVenues = Data.VenueManager.GetVenuesWithCoordinates();
        _bubbles.datas = new UIBubblePopup.CData[allVenues.Count];
        for (var i = 0; i < _bubbles.datas.Length; i++)
        {
            _bubbles.datas[i] = ConverToOnlineMaps(allVenues[i]);
        }
    }

    private void OnDisable()
    {
        _map.gameObject.SetActive(false);
        _canvasPopup.gameObject.SetActive(false);
    }

    private UIBubblePopup.CData ConverToOnlineMaps(VenueModel venue) 
    {
        var data = new UIBubblePopup.CData();   
        data.title = venue.Name;
        data.address = venue.Location.Address;
        data.latitude = venue.Location.Latitude;
        data.longitude = venue.Location.Longitude;
        return data;
    }
}
