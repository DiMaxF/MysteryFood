using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

public class VenueScreen : AppScreen
{
    [Header("Toolbar")]
    [SerializeField] private ButtonView _editButton;
    [SerializeField] private ButtonView _backButton;
    [SerializeField] private Text _name;
    [Header("Content")]
    [SerializeField] private AsyncImageView _image;
    [SerializeField] private Text _location;
    [SerializeField] private ButtonView _phone;
    [SerializeField] private Text _description;
    [SerializeField] private Text _ingridientsAllergenes;
    [SerializeField] private Text _price;
    [SerializeField] private ButtonView _viewOnMap;
    [SerializeField] private ButtonView _reserveForPickup;
    [Header("Overlay")]
    [SerializeField] private ConfirmPanel _confirmPanel;
    [SerializeField] private EnterCoordinatesView _enterCoordinates;
    [SerializeField] private ToastView _toast;
    private VenueModel _model;

    protected override void OnStart()
    {
        base.OnStart();
        UIContainer.RegisterView(_confirmPanel);
        UIContainer.RegisterView(_enterCoordinates);
        UIContainer.RegisterView(_toast);
        _enterCoordinates.Hide();
        _confirmPanel.Hide();
        _toast.Hide();
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        if (_model == null) return;
        _name.text = _model.Name;
        _location.text = _model.Location.Address;
        UIContainer.InitView(_phone, _model.Phone);
        UIContainer.InitView(_image, _model.ImagePath);
        _description.text = _model.Description;
        _ingridientsAllergenes.text = _model.IngredientsAllergens;
        _price.text = _model.Price.ToString();
        _name.text = _model.Name;

    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView(_backButton, (object _)  => OnButtonBack());
        UIContainer.SubscribeToView(_viewOnMap, (object _)  => OnButtonViewOnMap());
        UIContainer.SubscribeToView(_editButton, (object _)  => OnButtonEdit());
        UIContainer.SubscribeToView(_phone, (object _) => OnButtonDialer());
        UIContainer.SubscribeToView<ConfirmPanel, bool>(_confirmPanel, OnConfirmPanelResult);
        UIContainer.SubscribeToView<EnterCoordinatesView, GeoPoint>(_enterCoordinates, EnterCoordinates);

        UIContainer.SubscribeToView(_reserveForPickup, (object _) => OnButtonReserveForPickup());
    }

    private void OnButtonBack()
    {
        Container.Back().Forget();
    }

    private void OnButtonDialer() 
    {
        PhoneDialer.OpenDialer(_phone.Text);
    }

    private void OnButtonEdit()
    {
        var screen = Container.GetScreen<AddVenueScreen>();
        screen.SetModel(_model);
        Container.Show(screen);
    }

    private void OnButtonReserveForPickup() 
    {
        var screen = Container.GetScreen<AddReservationScreen>();
        screen.SetVenue(_model);
        Container.Show(screen);
    }


    private void OnButtonViewOnMap() 
    {
        if (_model.Location.Latitude == 0)
        {
            _confirmPanel.Show();
            UIContainer.InitView(_confirmPanel, "No coordinates set for this venue. Add now?");
        }
        else 
        {
            var screen = Container.GetScreen<MapScreen>();  
            Container.Show(screen);
        }
    }

    private void OnConfirmPanelResult(bool val) 
    {
        if (val)
        {
            UIContainer.InitView(_enterCoordinates, _model.Location);    
            _enterCoordinates.Show();
        }
        _confirmPanel.Hide();
    }

    private void EnterCoordinates(GeoPoint geo) 
    {
        if (geo.Longitude != 0) 
        {
            _model.Location.Latitude = geo.Latitude;
            _model.Location.Longitude = geo.Longitude;
            Data.VenueManager.UpdateVenue(_model);
            Data.SaveData();
            _toast.Show();
            UIContainer.InitView(_toast, "Coordinates successfully saved");
        }
        _enterCoordinates.Hide();
    }

    public void SetModel(VenueModel venue) => _model = venue;

}
