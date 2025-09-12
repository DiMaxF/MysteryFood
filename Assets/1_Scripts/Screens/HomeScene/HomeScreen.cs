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
        UIContainer.SubscribeToView<ListView, EventModel>(venues, OnEventsAction);
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        UIContainer.InitView(venues, Data.Events.GetAll());
    }

    private void OnButtonAddEvent() 
    {
        Container.Show<AddEventScreen>();
    }

    private void OnEventsAction(EventModel model)
    {
        Data.Personal.SetSelectedEvent(model);
        Data.SaveData();
        Container.Show<EventScreen>();
    }
}
