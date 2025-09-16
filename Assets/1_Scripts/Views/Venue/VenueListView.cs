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
        //DataCore.Instance.Personal.Gep
        base.Init(data);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView(action, (object _) => TriggerAction(_model));
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        if (_model != null) 
        {
            UIContainer.InitView(image, _model.ImagePath);
            price.text = _model.Price.ToString();
            pickupToday.text = $"{_model.StartTime}-{_model.EndTime}";
            name.text = _model.Name;
            address.text = _model.Location.Address;
            if (!DataCore.Instance.PersonalManager.PermissionLocation)
            {
                distance.text = "—";
            }
            else 
            {
                if (_model.Location.Latitude != 0)
                {
                    distance.text = $"{DataCore.Instance.PersonalManager.CalculateDistance(_model.Location):F2} km";
                    DataCore.Instance.PersonalManager.CalculateDistance(_model.Location);
                }
                else
                {

                    distance.text = "—";
                }
            }
        }
    }
}
