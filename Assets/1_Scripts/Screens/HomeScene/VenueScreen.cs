using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VenueScreen : AppScreen
{
    [Header("Toolbar")]
    [SerializeField] private ButtonView _editButton;
    [SerializeField] private ButtonView _backButton;
    [SerializeField] private Text _name;
    [Header("Content")]
    [SerializeField] private AsyncImageView _image;
    [SerializeField] private Text _location;
    [SerializeField] private Text _phone;
    [SerializeField] private Text _description;
    [SerializeField] private Text _ingridientsAllergenes;
    [SerializeField] private Text _price;
    [SerializeField] private ButtonView _viewOnMap;
    [SerializeField] private ButtonView _reserveForPickup;
}
