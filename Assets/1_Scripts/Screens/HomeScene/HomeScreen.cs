using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreen : AppScreen
{
    [SerializeField] SearchView searchView;
    [SerializeField] ListView venues;
    [SerializeField] BaseView hintDistance;
    [SerializeField] ButtonView addReservation;
    [SerializeField] ButtonView addVenue;
    [SerializeField] ButtonView savings;

    protected override void OnStart()
    {
        base.OnStart();
        Data.Personal.SetSelectedEvent(null);
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(addReservation, _ => OnButtonAddEvent());
        UIContainer.SubscribeToView<ButtonView, object>(addVenue, _ => OnButtonAddVenue());
        UIContainer.SubscribeToView<ListView, VenueModel>(venues, OnVenueAction);
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        UIContainer.InitView(venues, Data.VenueManager.GetAll());
    }
    private void OnButtonAddVenue()
    {
        Container.Show<AddVenueScreen>();
    }

    private void OnButtonAddEvent() 
    {
        Container.Show<AddEventScreen>();
    }

    private void OnVenueAction(VenueModel model)
    {
        var screen = Container.GetScreen<VenueScreen>();
        screen.SetModel(model);
        Container.Show(screen);
    }
}
