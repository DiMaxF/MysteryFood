using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class SelectVenueView : View
{
    [SerializeField] private ListView _list;
    [SerializeField] private ButtonView _cancel;
    [SerializeField] private ButtonView _continue;
    [SerializeField] private VenueModel _selectedVenue;

    private List<VenueToggleView.Data> _venues;
    private List<VenueToggleView.Data> GenerateData(List<VenueModel> venues) 
    {
        List<VenueToggleView.Data> list = new List<VenueToggleView.Data>();
        foreach (var v in venues) list.Add(new VenueToggleView.Data(v, _selectedVenue.Id== v.Id));
        return list;
    }

    private void UpdateData() 
    {
        for (var i = 0; i < _venues.Count; i++) 
        {
            var v = _venues[i];
            v = new VenueToggleView.Data(v.Model, _selectedVenue.Id == v.Model.Id);
        }
    }

    public override void Init<T>(T data)
    {
        if (data is List<VenueModel> venues) _venues = GenerateData(venues);
        UIContainer.RegisterView(_list);
        UIContainer.RegisterView(_cancel);
        UIContainer.RegisterView(_continue);
        base.Init(data);
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        //_continue.interactable = _selectedVenue != null;
        UIContainer.InitView(_list, _venues);
        
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ListView, VenueToggleView.Data>(_list, SelectVenue);
        UIContainer.SubscribeToView(_cancel, (object _) => TriggerAction((VenueModel)null));
        UIContainer.SubscribeToView(_continue, (object _) => TriggerAction(_selectedVenue));

    }

    private void SelectVenue(VenueToggleView.Data venue) 
    {
        if (venue != null)
        {
            _selectedVenue = venue.Model;

            UpdateData();
            UpdateUI();
        }
        else 
        {
            var screen = AppContainer.Instance.GetScreen<AddVenueScreen>();
            screen.SetModel(null);
            AppContainer.Instance.Show(screen);
            Hide();
        }

    }
}
