using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenueToggleView : View
{

    [Serializable]
    public class Data 
    {
        public VenueModel Model;
        public bool Active;

        public Data(VenueModel model, bool active) 
        {
            Model = model;
            Active = active;
        }
    }

    [SerializeField] private VenueListView _venue;
    [SerializeField] private ToggleView _toggle;

    [SerializeField] private Data _data;

    public override void Init<T>(T data)
    {
        if (data is Data tuple) _data = tuple;

        UIContainer.RegisterView(_venue);
        UIContainer.RegisterView(_toggle);
        base.Init(data);

    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        UIContainer.InitView(_venue, _data.Model);
        UIContainer.InitView(_toggle, _data.Active);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ToggleView, bool>(_toggle, active => SetActive() );
    }

    private void SetActive() 
    {
        TriggerAction(new Data(_data.Model, true));
    }
}
