using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TicketsScreen : AppScreen
{
    [SerializeField] ListView tickets;
    [SerializeField] SearchView search;
    [SerializeField] AsyncImageView qrCode;
    [SerializeField] ButtonView share;

    private List<TicketModel> d;
    private TicketModel ticket;
    protected override void OnStart()
    {
        var selected = Data.Personal.GetSelectedEvent();
        if (selected != null)
        {
            d = selected.tickets;
        }
        else
        {
        }
        UIContainer.InitView(search, d);
        base.OnStart();

    }
    protected override void UpdateViews()
    {
        base.UpdateViews();
        UIContainer.InitView(tickets, d);
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ListView, TicketModel>(tickets, ViewTicket);
        UIContainer.SubscribeToView<SearchView, List<TicketModel>>(search, Search);
        UIContainer.InitView(search, d);
        UIContainer.SubscribeToView<SearchView, object>(search, _ => { Logger.Log($"{typeof(object)}", "TicketsScreen"); });
        UIContainer.SubscribeToView<ButtonView, object>(share, _ => OnButtonShare());
        

    }

    private void Search(List<TicketModel> tickets) 
    {
        d = tickets;
        UpdateViews();
    }

    private void ViewTicket(TicketModel ticket) 
    {
        UIContainer.InitView(qrCode, ticket.qrPath);
        qrCode.Show();
        this.ticket = ticket;
    }

    private void OnButtonShare() 
    {

    }
}
