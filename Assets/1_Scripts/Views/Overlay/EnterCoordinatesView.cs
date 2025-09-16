using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterCoordinatesView : View
{
    [SerializeField] private InputTextView _latitude;
    [SerializeField] private InputTextView _longtitude;
    [SerializeField] private ButtonView _cancel;
    [SerializeField] private ButtonView _confirm;
    private GeoPoint _geo = new GeoPoint();

    public override void Init<T>(T data)
    {
        if (data is GeoPoint geo) _geo = geo;
        UIContainer.RegisterView(_cancel);
        UIContainer.RegisterView(_confirm);
        UIContainer.RegisterView(_latitude);
        UIContainer.RegisterView(_longtitude);
        UIContainer.InitView(_longtitude, _geo.Longitude == 0 ? "" : _geo.Longitude.ToString());
        UIContainer.InitView(_latitude, _geo.Latitude == 0 ? "" : _geo.Latitude.ToString());
        base.Init(data);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView(_cancel, (object _) => TriggerAction(new GeoPoint()));
        UIContainer.SubscribeToView(_confirm, (object _) => TriggerAction(_geo));
        UIContainer.SubscribeToView<InputTextView, string>(_latitude, OnLatitudeInput);
        UIContainer.SubscribeToView<InputTextView, string>(_longtitude, OnLongtitudeInput);
    }

    private void OnLatitudeInput(string val) 
    {
        Logger.Log($"Latitude input: {val}", "EnterCoordinatesView");   
        if (!float.TryParse(val, out var latitude)) return;
        _geo.Latitude = latitude;   
    }
    private void OnLongtitudeInput(string val)
    {
        if (!float.TryParse(val, out var longtitude)) return;
        _geo.Longitude = longtitude;
    }
}
