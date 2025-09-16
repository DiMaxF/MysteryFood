using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreen : AppScreen
{
    [SerializeField] SearchView searchView;
    [SerializeField] ListView venues;
    [SerializeField] ButtonView hintDistance;
    [SerializeField] ButtonView _noVenues;
    [SerializeField] ButtonView addReservation;
    [SerializeField] ButtonView addVenue;
    [SerializeField] ButtonView savings;

    [SerializeField] private SelectVenueView _selectVenue;

    protected override void OnStart()
    {
        base.OnStart();
        if (Data.VenueManager.GetAll().Count == 0)
        {
            _noVenues.Show();
        }
        else 
        {
            _noVenues.Hide();
        }
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.RegisterView(_selectVenue);
        UIContainer.SubscribeToView<SelectVenueView, VenueModel>(_selectVenue, SelectVenueToReservation);

        UIContainer.SubscribeToView<ButtonView, object>(addReservation, _ => OnButtonAddReservation());
        UIContainer.SubscribeToView<ButtonView, object>(addVenue, _ => OnButtonAddVenue());
        UIContainer.SubscribeToView<ButtonView, object>(_noVenues, _ => OnButtonAddVenue());
        UIContainer.SubscribeToView<ListView, VenueModel>(venues, OnVenueAction);
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        UIContainer.InitView(venues, Data.VenueManager.GetAll());
    }
    private void OnButtonAddVenue()
    {
        var screen = Container.GetScreen<AddVenueScreen>();
        screen.SetModel(null);
        Container.Show(screen);
    }

    private void OnButtonAddReservation() 
    {
        UIContainer.InitView(_selectVenue, Data.VenueManager.GetAll());
        _selectVenue.Show();

    }

    private void SelectVenueToReservation(VenueModel venue)
    {
        if (venue != null)
        {
            var screen = Container.GetScreen<AddReservationScreen>();
            screen.SetVenue(venue);
            Container.Show(screen);
        }
        else
        {

        }
        _selectVenue.Hide();
    }

    private void OnVenueAction(object obj)
    {
        if (!(obj is VenueModel model)) 
        {
            OnButtonAddVenue();
            return;
        }
        var screen = Container.GetScreen<VenueScreen>();
        screen.SetModel(model);
        Container.Show(screen);

    }
}
