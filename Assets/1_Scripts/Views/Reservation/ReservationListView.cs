using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReservationListView : View
{
    [SerializeField] private AsyncImageView _image;
    [SerializeField] private Text _venueName;
    [SerializeField] private Text _venueAddress;
    [SerializeField] private StatusView _status;
    [SerializeField] private Text _reservationId;
    [SerializeField] private Text _reservationTime;
    [SerializeField] private Text _quantity;
    [SerializeField] private Text _totalPrice;
    [Header("Dropdown")]
    [SerializeField] private BaseView _dropdown;
    [SerializeField] private ButtonView _hideDropdown;
    [SerializeField] private ButtonView _showDropdown;
    [SerializeField] private ButtonView _showQr;
    [SerializeField] private ButtonView _cancel;

    private ReservationModel _model;
    private DataCore _data => DataCore.Instance;

    public override void Init<T>(T data)
    {
        if (data is ReservationModel reservation) _model = reservation;
        UIContainer.RegisterView(_showDropdown);
        UIContainer.RegisterView(_hideDropdown);
        UIContainer.RegisterView(_showQr);
        UIContainer.RegisterView(_cancel);
        UIContainer.RegisterView(_dropdown);
        UIContainer.RegisterView(_status);
        _dropdown.Hide();
        base.Init(data);
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        var venue = _data.VenueManager.GetById(_model.VenueId);
        if (venue == null) return;
        UIContainer.InitView(_image, venue.ImagePath);
        UIContainer.InitView(_status, _model.Status);
        _venueName.text = venue.Name;
        _venueAddress.text = venue.Location.Address;
        _reservationId.text = $"ID-{_model.Id}";
        _reservationTime.text = $"{_model.StartTime}-{_model.EndTime} PM";
        _quantity.text = $"x{_model.Quantity}";
        _totalPrice.text = $"{_model.DiscountedPrice.Amount * _model.Quantity} {_model.DiscountedPrice.Currency}";
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView(_hideDropdown, (object _) => _dropdown.Hide());
        UIContainer.SubscribeToView(_showDropdown, (object _) => _dropdown.Show());
        UIContainer.SubscribeToView(_showQr, (object _) => OnButtonShow());
        UIContainer.SubscribeToView(_cancel, (object _) => OnButtonCancelled());

    }

    private void OnButtonCancelled() 
    {
        TriggerAction(("Cancel", _model));

    }

    private void OnButtonShow() 
    {
        TriggerAction(("Open", _model));

    }
}
