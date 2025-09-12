using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Text _saveMoney;
    [SerializeField] private Text _price;
    [SerializeField] private Text _quantity;
    [SerializeField] private InputTextView _notes;
}
