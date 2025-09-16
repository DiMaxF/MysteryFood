using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OrderDetailsScreen : AppScreen
{
    [SerializeField] private ButtonView _backButton;
    [SerializeField] private ButtonView _showQrButton;
    [SerializeField] private ButtonView _markedAsPickedButton;
    [SerializeField] private ButtonView _cancelReservationButton;
    [SerializeField] private BaseView _cancelledDate;
    [SerializeField] private BaseView _pickedDate;

    [Header("Venue")]
    [SerializeField] private Text _name;
    [SerializeField] private AsyncImageView _image;
    [SerializeField] private Text _location;
    [SerializeField] private ButtonView _phone;
    [Header("Reservation")]
    [SerializeField] private Text _quantity;
    [SerializeField] private Text _totalPrice;
    [SerializeField] private Text _savedMoney;
    [SerializeField] private Text _reservationId;
    [SerializeField] private StatusView _status;

    private ReservationModel _model;

    public void SetModel(ReservationModel model) => _model = model;

    protected override void UpdateViews()
    {
        base.UpdateViews();
        if (_model == null) return;
        var venue = Data.VenueManager.GetById(_model.VenueId);
        _name.text = venue.Name;
        _quantity.text = $"x{_model.Quantity}";
        _location.text = venue.Location.Address;
        UIContainer.InitView(_phone, venue.Phone);
        UIContainer.InitView(_image, venue.ImagePath);
        UIContainer.InitView(_status, _model.Status);
        _totalPrice.text = _model.DiscountedPrice.ToString();
        _savedMoney.text = $"{(_model.OriginalPrice.Amount - _model.DiscountedPrice.Amount) * _model.Quantity}{Data.PersonalManager.GetCurrency}";
        _reservationId.text = $"ID-{_model.Id}";
    }

    private void SetViewsByStatus(StatusReservation status) 
    {
        switch (status)
        {
            case StatusReservation.Booked:

                break;
            case StatusReservation.Cancelled:
                break;
            case StatusReservation.PickedUp:
                _pickedDate.Show();
                _cancelReservationButton.Hide();
                _markedAsPickedButton.Hide();
                _cancelledDate.Hide();
                break;
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView(_phone, (object _) => OnButtonDialer());
        UIContainer.SubscribeToView(_backButton, (object _) => OnButtonBack());
        UIContainer.SubscribeToView(_showQrButton, (object _) => OnButtonQr());
        UIContainer.SubscribeToView(_markedAsPickedButton, (object _) => OnUpdateStatus(StatusReservation.PickedUp));
        UIContainer.SubscribeToView(_cancelReservationButton, (object _) => OnUpdateStatus(StatusReservation.PickedUp));

    }

    private void OnButtonBack()
    {
        Container.Back().Forget();
    }

    private void OnButtonDialer()
    {
        PhoneDialer.OpenDialer(_phone.Text);
    }
    private void OnButtonQr()
    {
        var screen = Container.GetScreen<QrReservationScreen>();
        screen.SetModel(_model);
        Container.Show(screen);
    }

    private void OnUpdateStatus(StatusReservation status)
    {
        _model.Status = status;
        Data.ReservationManager.Update(_model);
        Data.SaveData();
    }
}
