using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdersScreen : AppScreen
{

    [SerializeField] private SearchView _searchView;
    [SerializeField] private ToggleView _active;
    [SerializeField] private ToggleView _past;
    [SerializeField] private ToggleView _cancelled;
    [SerializeField] private ListView _list;
    [SerializeField] private ConfirmPanel _confirm;
    [SerializeField] private SelectVenueView _selectVenue;

    private StatusReservation _showCategory = StatusReservation.Booked;

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.RegisterView(_selectVenue);
        UIContainer.SubscribeToView<SelectVenueView, VenueModel>(_selectVenue, SelectVenueToReservation);
        UIContainer.SubscribeToView<ListView, (string, ReservationModel)>(_list, ListAction);
        UIContainer.SubscribeToView<ToggleView, bool>(_active, val => SetCategory(StatusReservation.Booked));
        UIContainer.SubscribeToView<ToggleView, bool>(_past, val => SetCategory(StatusReservation.PickedUp));
        UIContainer.SubscribeToView<ToggleView, bool>(_cancelled, val => SetCategory(StatusReservation.Cancelled));
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        UpdateToggles();
        UIContainer.InitView(_list, Data.ReservationManager.GetSorted(_showCategory));
    }
    private void SetCategory(StatusReservation category) 
    {
        _showCategory = category;
        UpdateViews();
    }


    private void UpdateToggles() 
    {
        UIContainer.InitView(_active, _showCategory == StatusReservation.Booked);
        UIContainer.InitView(_past, _showCategory == StatusReservation.PickedUp);
        UIContainer.InitView(_cancelled, _showCategory == StatusReservation.Cancelled);
    }

    private void ListAction((string, ReservationModel) action) 
    {
        if (action.Item1 == "Open")
        {
            var screen = Container.GetScreen<QrReservationScreen>();
            screen.SetModel(action.Item2);
            Container.Show(screen);
        }
        else if (action.Item1 == "Cancel")
        {
            action.Item2.Status = StatusReservation.Cancelled;
            Data.ReservationManager.Update(action.Item2);
            Data.SaveData();
        }
        else { OnButtonAddReservation(); }
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

}
