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
    public string ShortString(string input, int length)
    {
        if (string.IsNullOrEmpty(input) || length <= 0)
            return string.Empty;

        if (input.Length <= length)
            return input;

        return input.Substring(0, length) + "...";
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
            name.text =ShortString(_model.Name, 40);
        }

        if (address != null && _model.Location != null)
        {
            address.text = ShortString(_model.Location.Address, 40);
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