using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapFiltersView : View
{
    [SerializeField] private ButtonView _applyFilters;
    [SerializeField] private ButtonView _reset;
    [SerializeField] private ButtonView _close;
    [SerializeField] private SliderView _distance;

    [Header("Toggles Date")]
    [SerializeField] private InputTextView _minPrice;
    [SerializeField] private InputTextView _maxPrice;
    [Header("Other")]
    [SerializeField] private ToggleView _pickupNow;

    private DataCore _data => DataCore.Instance;
    private FilterOptions _filters;

    public override void Init<T>(T data)
    {
        if (data is FilterOptions filters) _filters = filters;

        UIContainer.RegisterView(_applyFilters);
        UIContainer.RegisterView(_reset);
        UIContainer.RegisterView(_close);
        UIContainer.RegisterView(_distance);
        UIContainer.RegisterView(_pickupNow);
        UIContainer.RegisterView(_minPrice);
        UIContainer.RegisterView(_maxPrice);

        base.Init(data);
        UIContainer.InitView(_distance, new SliderData(0, 10));
        UIContainer.InitView(_minPrice, "");
        UIContainer.InitView(_maxPrice, "");
        UIContainer.InitView(_pickupNow, false);

    }

    public override void Subscriptions()
    {
        base.Subscriptions();

        UIContainer.SubscribeToView<ButtonView, object>(_applyFilters, _ => TriggerAction(_filters));
        UIContainer.SubscribeToView<ButtonView, object>(_close, _ => Hide());

        UIContainer.SubscribeToView<ToggleView, bool>(_pickupNow, NowToggle);
        UIContainer.SubscribeToView<InputTextView, string>(_minPrice, ChangeMinPrice);
        UIContainer.SubscribeToView<InputTextView, string>(_maxPrice, ChangMaxPrice);
        UIContainer.SubscribeToView<SliderView, float>(_distance, ChangeDistance);
    }
    private void NowToggle(bool val)
    {
        _filters.OnlyOpenNow = val;
    }
    private void ChangeMinPrice(string val) 
    {
        if (val == "") _filters.MinPrice = null;
        else 
        {
            if (!int.TryParse(val, out var price)) return;
            _filters.MinPrice = price;
        }

    }
    private void ChangMaxPrice(string val)
    {
        if (val == "") _filters.MaxPrice = null;
        else 
        {
            if (!int.TryParse(val, out var price)) return;
            _filters.MaxPrice = price;
        }
    }

    private void ChangeDistance(float val) 
    {
        if (val == 0) _filters.MaxDistanceKm = null;
        else _filters.MaxDistanceKm = val;
    }


    public override void UpdateUI()
    {
        base.UpdateUI();


    }


}
