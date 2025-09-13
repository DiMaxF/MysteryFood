using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle;
using Unity.VisualScripting;
using UnityEngine;

public class AddVenueScreen : AppScreen
{
    [SerializeField] private ButtonView _create;
    [SerializeField] private ButtonView _back;

    [Header("Image")]
    [SerializeField] private ButtonView _pickImage;
    [SerializeField] private ButtonView _repickImage;

    [SerializeField] private AsyncImageView _image;
    [Header("Time")]
    [SerializeField] private TimePickerView _timePicker;
    [SerializeField] private ButtonView _timeStart;
    [SerializeField] private InputTextView _timeStartInput;
    [SerializeField] private ButtonView _timeEnd;
    [SerializeField] private InputTextView _timeEndInput;

    [Header("Inputs")]
    [SerializeField] private InputTextView _name;
    [SerializeField] private InputTextView _address;
    [SerializeField] private InputTextView _phone;
    [SerializeField] private InputTextView _price;
    [SerializeField] private InputTextView _lattitude;
    [SerializeField] private InputTextView _longitude;
    [SerializeField] private InputTextView _description;
    [SerializeField] private InputTextView _ingredientsAllergens;

    private VenueModel _model;

    private bool _updateVenue;

    protected override void OnStart()
    {
        if (_model == null) 
        {
            _model = new VenueModel();
            _updateVenue = false;
        }
        else _updateVenue = true;
        base.OnStart();
        UIContainer.RegisterView(_timePicker);
        _timePicker.Hide();
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(_create, _ => OnButtonCreate());
        UIContainer.SubscribeToView<ButtonView, object>(_back, _ => OnButtonBack());
        UIContainer.SubscribeToView<ButtonView, object>(_pickImage, _ => OnButtonFilePick());
        UIContainer.SubscribeToView<ButtonView, object>(_repickImage, _ => OnButtonFilePick());

        UIContainer.SubscribeToView<ButtonView, object>(_timeStart, _ => OnButtonTimeStart());
        UIContainer.SubscribeToView<ButtonView, object>(_timeEnd, _ => OnButtonTimeEnd());

        UIContainer.SubscribeToView<InputTextView, string>(_name, OnNameEdit);
        UIContainer.SubscribeToView<InputTextView, string>(_description, OnNameEdit);
        UIContainer.SubscribeToView<InputTextView, string>(_phone, OnPhoneEdit);
        UIContainer.SubscribeToView<InputTextView, string>(_price, OnPriceEdit);
        UIContainer.SubscribeToView<InputTextView, string>(_address, OnAddressEdit);
        UIContainer.SubscribeToView<InputTextView, string>(_description, OnDescriptionEdit);
        UIContainer.SubscribeToView<InputTextView, string>(_ingredientsAllergens, OnIngredientsAllergensEdit);



        UIContainer.SubscribeToView<InputTextView, string>(_lattitude, OnCoordinatesEdit);
        UIContainer.SubscribeToView<InputTextView, string>(_longitude, OnCoordinatesEdit);
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        UIContainer.InitView(_name, _model.Name);
        UIContainer.InitView(_image, _model.ImagePath);
        UIContainer.InitView(_address, _model.Location.Address);
        UIContainer.InitView(_phone, _model.Phone.ToString());
        UIContainer.InitView(_price, _model.Price.ToString());
        UIContainer.InitView(_lattitude, _model.Location.Latitude);
        UIContainer.InitView(_longitude, _model.Location.Longitude);
        UIContainer.InitView(_description, _model.Description);
        UIContainer.InitView(_ingredientsAllergens, _model.IngredientsAllergens);

        UIContainer.InitView(_timeStartInput, _model.StartTime);
        UIContainer.InitView(_timeEndInput, _model.EndTime);
        if (_model.ImagePath != "")
        {
            _pickImage.Hide();
            _repickImage.Show();
        }
        else 
        {
            _pickImage.Show();
            _repickImage.Hide();
        }
    }

    public void SetModel(VenueModel venue) => _model = venue;

    #region ViewsActions

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

    private void OnPhoneEdit(string val)
    {
        if (!int.TryParse(val, out var phone)) return;
        _model.Phone = phone;
        ValidateModel();
    }
    private void OnPriceEdit(string val)
    {
        if (!int.TryParse(val, out var price)) return;
        _model.Price = new CurrencyModel(price, Data.Personal.GetCurrency());
        ValidateModel();
    }
    private void OnDescriptionEdit(string val)
    {
        _model.Description = val;
        ValidateModel();
    }

    private void OnIngredientsAllergensEdit(string val)
    {
        _model.IngredientsAllergens = val;
        ValidateModel();
    }

    private void OnNameEdit(string val)
    {
        _model.Name = val;
        ValidateModel();
    }
    private void OnAddressEdit(string val)
    {
        var coordinates = GetCoordinates();
        _model.Location = new GeoPoint(coordinates, val);
        ValidateModel();
    }

    private void OnCoordinatesEdit(string val)
    {
        _model.Location = new GeoPoint(GetCoordinates(), _address.text);
        ValidateModel();
    }


    private void OnButtonCreate()
    {
        if (ValidateModel()) 
        {
            if (_updateVenue) Data.VenueManager.UpdateVenue(_model);
            else Data.VenueManager.AddVenue(_model);
            Data.SaveData();
        }
    }

    private void OnButtonBack()
    {
        Container.Back().Forget();
    }


    private void OnButtonFilePick() 
    {
        NativeGallery.GetImageFromGallery(async (path) =>
        {
            if (!string.IsNullOrEmpty(path))
            {
                var selectedImagePath = await FileManager.SaveImage(path);
                if (!string.IsNullOrEmpty(selectedImagePath))
                {
                    UIContainer.InitView(_image, selectedImagePath);
                    _model.ImagePath = selectedImagePath;
                }
                else
                {
                    Logger.LogError("Failed to save image, selectedImagePath is null");
                }
            }
            else
            {
                Logger.LogWarning("No file selected");
            }
        }, "Select Image", "image/*");
    }

    #endregion

    private Vector2 GetCoordinates()
    {
        if (!float.TryParse(_lattitude.text, out var lattitude)) return Vector2.zero;
        if (!float.TryParse(_longitude.text, out var longitude)) return Vector2.zero;
        return new Vector2(lattitude, longitude);
    }

    private bool ValidateModel()
    {
        _name.DefaultColor();
        _address.DefaultColor();
        _phone.DefaultColor();
        if (_name.text == "") 
        {
            return InputError(_name);
        }
        if (_address.text == "")
        {
            return InputError(_address);
        }
        if (_phone.text == "" || _phone.text.Length < 11)
        {
            return InputError(_phone);
        }
        if (_timeStartInput.text == "" || !TimeSpan.TryParse(_timeStartInput.text, out var startTime))
        {
            return InputError(_timeStartInput);
        }
        if (_timeEndInput.text == "" || !TimeSpan.TryParse(_timeStartInput.text, out var endTime))
        {
            return InputError(_timeEndInput);
        }

        if (startTime > endTime)
        {
            InputError(_timeStartInput);
            return InputError(_timeEndInput);
        }
        _create.interactable = true;
        return true;
    }

    private bool InputError(InputTextView input) 
    {
        _create.interactable = false;
        input.HighlightError();
        return false;
    }
}
