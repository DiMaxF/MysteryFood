using Cysharp.Threading.Tasks;
using System;
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
    [Header("DateTimes")]
    [SerializeField] private Text _pickedDateText;
    [SerializeField] private Text _cancelledDateText;
    [SerializeField] private Text _createdAtText;
    [SerializeField] private Text _reminderSchedule;
    [Header("Venue")]
    [SerializeField] private Text _name;
    [SerializeField] private AsyncImageView _image;
    [SerializeField] private Text _location;
    [SerializeField] private ButtonView _phone;
    [SerializeField] private Text _pickupWindow;
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
        _savedMoney.text = $"{(_model.OriginalPrice.Amount - _model.DiscountedPrice.Amount) * _model.Quantity}{Data.PersonalManager.Currency}";
        _reservationId.text = $"ID-{_model.Id}";
        _pickedDateText.text = _model.EndAt;
        _cancelledDateText.text = _model.EndAt;
        _createdAtText.text = _model.CreatedAt;
        _reminderSchedule.text = Data.PersonalManager.Notification > 0 ?  $"{Data.PersonalManager.Notification} min before pickup" : "None";
        SetViewsByStatus(_model.Status);
        _pickupWindow.text = $"{_model.StartTime}-{_model.EndTime}";
    }

    private void SetViewsByStatus(StatusReservation status) 
    {
        switch (status)
        {
            case StatusReservation.Booked:
                _pickedDate.Hide();
                _cancelReservationButton.Show();
                _markedAsPickedButton.Show();
                _cancelledDate.Hide();
                _showQrButton.Show();

                break;
            case StatusReservation.Cancelled:
                _pickedDate.Show();
                _cancelReservationButton.Hide();
                _markedAsPickedButton.Hide();
                _cancelledDate.Hide();
                _showQrButton.Hide();
                break;
            case StatusReservation.PickedUp:
                _pickedDate.Show();
                _cancelReservationButton.Hide();
                _markedAsPickedButton.Hide();
                _cancelledDate.Hide();
                _showQrButton.Show();
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
        UIContainer.SubscribeToView(_cancelReservationButton, (object _) => OnUpdateStatus(StatusReservation.Cancelled));

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
        _model.EndAt = DateTime.Now.ToString(DateTimeUtils.Full);
        Data.ReservationManager.Update(_model);
        Data.SaveData();
    }
}
