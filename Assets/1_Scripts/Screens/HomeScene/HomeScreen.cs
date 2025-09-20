using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreen : AppScreen
{
    [SerializeField] InputTextView searchView;
    [SerializeField] ListView venues;
    [SerializeField] ButtonView hintDistance;
    [SerializeField] ButtonView addReservation;
    [SerializeField] ButtonView addVenue;
    [SerializeField] ButtonView savings;

    [SerializeField] private SelectVenueView _selectVenue;
    private string _searchData = "";
    protected override void OnStart()
    {
        base.OnStart();
        if (Data.PersonalManager.PermissionLocation)
        {
            hintDistance.Hide();
        }
        else 
        {
            hintDistance.Show();
        }
        UIContainer.InitView(searchView, "");
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.RegisterView(_selectVenue);
        UIContainer.SubscribeToView<SelectVenueView, VenueModel>(_selectVenue, SelectVenueToReservation);

        UIContainer.SubscribeToView<ButtonView, object>(addReservation, _ => OnButtonAddReservation());
        UIContainer.SubscribeToView<ButtonView, object>(addVenue, _ => OnButtonAddVenue());
        UIContainer.SubscribeToView<ButtonView, object>(savings, _ => OnButtonSavings());
        UIContainer.SubscribeToView<ButtonView, object>(hintDistance, _ => RequestLocationPermission() );
        //UIContainer.SubscribeToView<ButtonView, object>(_noVenues, _ => OnButtonAddVenue());
        UIContainer.SubscribeToView<ListView, VenueModel>(venues, OnVenueAction);
        UIContainer.SubscribeToView<InputTextView, string>(searchView, OnSearchViewAction);
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        var list = _searchData == "" ? Data.VenueManager.GetAll() : Data.VenueManager.SearchVenues(_searchData, Data.VenueManager.GetAll());  
        UIContainer.InitView(venues, list);
    }

    private async void RequestLocationPermission() 
    {
        var result = await Data.RequestLocationPermission();
        if(result) hintDistance.Hide();
        else hintDistance.Show();
        //hintDistance.Hide();
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

    private void OnButtonSavings()
    {
        Container.Show<SavingsTrackerSreen>();

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

    private void OnSearchViewAction(string val) 
    {
        _searchData = val;  
        UpdateViews();
    }
}
