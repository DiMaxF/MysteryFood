using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VenueListView : View
{
    [SerializeField] private AsyncImageView image;
    [SerializeField] private ButtonView action;
    [SerializeField] private Text address;
    [SerializeField] private Text name;
    [SerializeField] private Text price;
    [SerializeField] private Text distance;
    [SerializeField] private Text pickupToday;

    private VenueModel _model;

    public override void Init<T>(T data)
    {
        if (data is VenueModel venue)
        {
            _model = venue;
        }
        base.Init(data);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        if (action != null)
        {
            UIContainer.SubscribeToView(action, (object _) => TriggerAction(_model));
        }
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        if (_model == null) return;

        if (image != null)
        {
            UIContainer.InitView(image, _model.ImagePath);
        }

        if (price != null)
        {
            price.text = _model.Price.ToString();
        }

        if (pickupToday != null)
        {
            pickupToday.text = $"{_model.StartTime}-{_model.EndTime}";
        }

        if (name != null)
        {
            name.text = _model.Name;
        }

        if (address != null && _model.Location != null)
        {
            address.text = _model.Location.Address;
        }

        if (distance != null)
        {
            if (!DataCore.Instance.PersonalManager.PermissionLocation || _model.Location == null || _model.Location.Latitude == 0)
            {
                distance.text = "—";
            }
            else
            {
                distance.text = $"{DataCore.Instance.PersonalManager.CalculateDistance(_model.Location):F2} km";
            }
        }
    }
}