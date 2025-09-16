using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ZXing.QrCode.Internal;
using static OnlineMapsGPXObject;

public class AddReservationScreen : AppScreen
{
    [SerializeField] private ButtonView _back;
    [SerializeField] private ButtonView _confirm;
    [Header("Time")]
    [SerializeField] private TimePickerView _timePicker;
    [SerializeField] private ButtonView _timeStart;
    [SerializeField] private InputTextView _timeStartInput;
    [SerializeField] private ButtonView _timeEnd;
    [SerializeField] private InputTextView _timeEndInput;

    [Header("Venue")]
    [SerializeField] private Text _name;
    [SerializeField] private Text _address;
    [SerializeField] private Text _distance;

    [Header("Inputs")]
    [SerializeField] private InputTextView _originalPrice;
    [SerializeField] private InputTextView _discountedPrice;
    [SerializeField] private ManualCounterView _quantity;
    [SerializeField] private Text _saveMoney;
    [SerializeField] private Text _price;
    [SerializeField] private InputTextView _notes;

    [Header("QR")]
    [SerializeField] private QRCodeEncodeController _qrCodeEncodeController;

    private VenueModel _venueModel;
    private ReservationModel _model;

    public void SetVenue(VenueModel venue) => _venueModel = venue;

    protected override void OnStart()
    {
        if (_model == null)
        {
            _model = new ReservationModel();
            _model.VenueId = _venueModel.Id;    
        }
        _qrCodeEncodeController.onQREncodeFinished.RemoveAllListeners();
        _qrCodeEncodeController.onQREncodeFinished.AddListener(
            (texture) =>
            {
                HadnleTextureQR(texture);
            }
        );
        base.OnStart();
        UIContainer.RegisterView(_timePicker);
        _timePicker.Hide();
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(_confirm, _ => OnButtonConfirm());
        UIContainer.SubscribeToView<ButtonView, object>(_back, _ => OnButtonBack());

        UIContainer.SubscribeToView<ButtonView, object>(_timeStart, _ => OnButtonTimeStart());
        UIContainer.SubscribeToView<ButtonView, object>(_timeEnd, _ => OnButtonTimeEnd());

        UIContainer.SubscribeToView<ManualCounterView, int>(_quantity, OnQuantityChange);

        UIContainer.SubscribeToView<InputTextView, string>(_originalPrice, OnOriginalPriceEdit);
        UIContainer.SubscribeToView<InputTextView, string>(_discountedPrice, OnDiscountedPriceEdit);
        UIContainer.SubscribeToView<InputTextView, string>(_notes, OnNotesEdit);

    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        _name.text = _venueModel.Name;
        _address.text = _venueModel.Location.Address;

        _distance.text = "—";

        UIContainer.InitView(_quantity, _model.Quantity);

        UIContainer.InitView(_timeStartInput, _model.StartTime);
        UIContainer.InitView(_timeEndInput, _model.EndTime);

        UIContainer.InitView(_discountedPrice, _model.DiscountedPrice.ToString());
        UIContainer.InitView(_originalPrice, _model.OriginalPrice.ToString());
        UIContainer.InitView(_notes, _model.Notes);

    }

    private void UpdatePrices() 
    {
        if (!int.TryParse(_originalPrice.text, out var orPrice)) 
        {
            _saveMoney.text = "—";
            _price.text = "—";
            return;
        }
        
        if (!int.TryParse(_discountedPrice.text, out var disPrice))
        {
            _saveMoney.text = "—";
            _price.text = "—";
            return;
        }
        _saveMoney.text = $"{(orPrice - disPrice) * _model.Quantity}";
        _price.text = $"{disPrice * _model.Quantity}";
    }

    #region ViewsActions

    private void OnOriginalPriceEdit(string value)
    {
        if (!int.TryParse(value, out var price)) return;
        _model.OriginalPrice = new CurrencyModel(price, Currency.USD);
        ValidateModel();
        UpdatePrices();
    }
    private void OnDiscountedPriceEdit(string value)
    {
        if (!int.TryParse(value, out var price)) return;
        _model.DiscountedPrice = new CurrencyModel(price, Currency.USD);
        ValidateModel();
        UpdatePrices();
    }

    private void OnQuantityChange(int quantity)
    {
        _model.Quantity = quantity;
        UpdatePrices();
    }

    private void OnNotesEdit(string value)
    {
        _model.Notes = value;
    }

    private void OnButtonTimeStart()
    {
        UIContainer.UnsubscribeFromView(_timePicker);
        UIContainer.SubscribeToView<TimePickerView, string>(_timePicker, OnTimeSaveStart);
        _timePicker.Show();
        UIContainer.InitView(_timePicker, _model.StartTime);
    }

    private void OnButtonTimeEnd()
    {
        UIContainer.UnsubscribeFromView(_timePicker);
        UIContainer.SubscribeToView<TimePickerView, string>(_timePicker, OnTimeSaveEnd);
        _timePicker.Show();
        UIContainer.InitView(_timePicker, _model.EndTime);
    }

    private void OnTimeSaveEnd(string time)
    {
        if (TimeSpan.TryParse(time, out var t)) _model.EndTime = time;
        _timePicker.Hide();
        UpdateViews();
        ValidateModel();
    }

    private void OnTimeSaveStart(string time)
    {
        if (TimeSpan.TryParse(time, out var t)) _model.StartTime = time;
        _timePicker.Hide();
        UpdateViews();
        ValidateModel();
    }

    private void OnButtonBack()
    {
        Container.Back().Forget();
    }

    private void SaveWithQr(string qrPath) 
    {
        _model.CreatedAt = DateTime.Now.ToString(DateTimeUtils.Full);
        _model.QrPath = qrPath;
        Data.ReservationManager.Add(_model);
        Data.SaveData();


        var screen = Container.GetScreen<QrReservationScreen>();
        screen.SetModel(_model);
        Container.Show(screen);
    }

    private void OnButtonConfirm()
    {
        if (ValidateModel())
        {
            var hash = $"ID-{_model.Id}, Address: {_venueModel.Location.Address}, Time: {_model.StartTime}-{_model.EndTime}";
            int errorlog = _qrCodeEncodeController.Encode(hash);
            //wait while saving qr, go ti SaveWithjQr
        }
    }

    #endregion


    private bool ValidateModel()
    {
        _timeStartInput.DefaultColor();
        _timeEndInput.DefaultColor();
        _originalPrice.DefaultColor();
        _discountedPrice.DefaultColor();
        if (_timeStartInput.text == "" || !TimeSpan.TryParse(_timeStartInput.text, out var startTime))
        {
            return InputError(_timeStartInput);
        }
        if (_timeEndInput.text == "" || !TimeSpan.TryParse(_timeStartInput.text, out var endTime))
        {
            return InputError(_timeEndInput);
        }

        if (startTime.Hours > endTime.Hours) 
        {
            InputError(_timeStartInput);
            return InputError(_timeEndInput);
        } 
        if (_originalPrice.text == "" || !int.TryParse(_originalPrice.text, out var orPrice))
        {
            return InputError(_originalPrice);
        }
        if (_discountedPrice.text == "" || !int.TryParse(_originalPrice.text, out var disPrice))
        {
            return InputError(_originalPrice);
        }
        if (orPrice < disPrice) 
        {
            InputError(_originalPrice);
            return InputError(_originalPrice);
        }
        _confirm.interactable = true;
        return true;
    }

    private bool InputError(InputTextView input)
    {
        _confirm.interactable = false;
        input.HighlightError();
        return false;
    }

    private async void HadnleTextureQR(Texture2D tex)
    {
        if (tex != null)
        {
            int width = tex.width;
            int height = tex.height;
            float aspect = width * 1.0f / height;
            var currentQRPath = await FileManager.SaveTexture(tex, "qr_ticket");
            SaveWithQr(currentQRPath);
        }
    }
}
