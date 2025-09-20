using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReservationShortView : View
{
    [SerializeField] private AsyncImageView _image;
    [SerializeField] private Text _venueName;
    [SerializeField] private Text _reservationId;
    [SerializeField] private Text _reservationTime;
    [SerializeField] private Text _quantity;
    [SerializeField] private Text _totalPrice;
    [SerializeField] private BaseView _contentToHide;

    private ReservationModel _model;
    private DataCore _data => DataCore.Instance;

    public override void Init<T>(T data)
    {
        if (data is ReservationModel reservation) _model = reservation;
        base.Init(data);
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        var venue = _data.VenueManager.GetById(_model.VenueId);
        if (venue == null) return;
        UIContainer.InitView(_image, venue.ImagePath);
        _venueName.text = venue.Name;

        _reservationId.text = $"ID-{_model.Id}";
        if (_model.Status == StatusReservation.Cancelled)
        {
            _contentToHide.Hide();
            _venueName.text = "Deleted";
            _venueName.color = Color.red;
        }
        else 
        {
            _reservationTime.text = $"{_model.StartTime}-{_model.EndTime} PM";
            _quantity.text = $"x{_model.Quantity}";
            _totalPrice.text = $"{_model.DiscountedPrice.Amount * _model.Quantity} {_model.DiscountedPrice.Currency}";
        }

    }
    
}
