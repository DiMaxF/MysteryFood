using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

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

    private VenueModel _model;

    protected override void OnStart()
    {
        base.OnStart();
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
        UIContainer.SubscribeToView(_editButton, (object _)  => OnButtonEdit());
    }

    private void OnButtonBack()
    {
        Container.Back().Forget();
    }

    private void OnButtonEdit()
    {
        var screen = Container.GetScreen<AddVenueScreen>();
        screen.SetModel(_model);
        Container.Show(screen);
    }

    public void SetModel(VenueModel venue) => _model = venue;

}
