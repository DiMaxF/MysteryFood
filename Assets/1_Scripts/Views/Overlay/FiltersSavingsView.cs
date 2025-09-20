using System;
using System.Collections.Generic;
using UnityEngine;

public class FiltersSavingsView : View
{
    [SerializeField] private ButtonView _applyFilters;
    [SerializeField] private ButtonView _reset;
    [SerializeField] private ButtonView _close;

    [Header("Toggles Date")]
    [SerializeField] private ToggleView _allTime;
    [SerializeField] private ToggleView _month;
    [SerializeField] private ToggleView _3months;
    [SerializeField] private ToggleView _customDateRange;
    [SerializeField] private DatePickerView _dateRange;
    [Header("Other")]
    [SerializeField] private ListView _venuesToggles;
    [SerializeField] private ToggleView _includedCancelled;
    [SerializeField] private ToggleView _includedBooked;

    private DateRanges _selectedDateRange;
    private DataCore _data => DataCore.Instance;
    [SerializeField] private FilterOptions _filters;

    public override void Init<T>(T data)
    {
        if (data is FilterOptions filters) _filters = filters;
        _selectedDateRange = DateRanges.AllTime;
        UIContainer.RegisterView(_allTime);
        UIContainer.RegisterView(_month);
        UIContainer.RegisterView(_3months);
        UIContainer.RegisterView(_customDateRange);
        UIContainer.RegisterView(_dateRange);
        UIContainer.RegisterView(_applyFilters);
        UIContainer.RegisterView(_reset);
        UIContainer.RegisterView(_close);
        UIContainer.RegisterView(_venuesToggles);
        UIContainer.RegisterView(_includedCancelled);
        UIContainer.RegisterView(_includedBooked);
        base.Init(data);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();

        UIContainer.SubscribeToView<ButtonView, object>(_applyFilters, _ => TriggerAction(_filters));
        UIContainer.SubscribeToView<ButtonView, object>(_close, _ => Hide());

        UIContainer.SubscribeToView<ToggleView, bool>(_allTime, (val) => SetDateRange(val, DateRanges.AllTime));
        UIContainer.SubscribeToView<ToggleView, bool>(_month, (val) => SetDateRange(val, DateRanges.Month));
        UIContainer.SubscribeToView<ToggleView, bool>(_3months, (val) => SetDateRange(val, DateRanges.ThreeMonths));
        UIContainer.SubscribeToView<ToggleView, bool>(_customDateRange, (val) => SetDateRange(val, DateRanges.Custom));


        UIContainer.SubscribeToView<ToggleView, bool>(_includedCancelled, (val) => AddCategory(val, StatusReservation.Cancelled));
        UIContainer.SubscribeToView<ToggleView, bool>(_includedBooked, (val) => AddCategory(val, StatusReservation.Booked));
        UIContainer.SubscribeToView<ListView, VenueToggleView.Data>(_venuesToggles, VenueSelect);

        UIContainer.SubscribeToView<DatePickerView, (string, string)>(_dateRange, CustomDateRange);
    }

    private void CustomDateRange((string, string) dates) 
    {
        _filters.FromDate = DateTime.Parse(dates.Item1);    
        _filters.ToDate = DateTime.Parse(dates.Item2);    
    }

    private void VenueSelect(VenueToggleView.Data select) 
    {
        _filters.VenueId = select.Model.Id;
        UIContainer.InitView(_venuesToggles, GenerateData((_data.VenueManager.GetAll())));
    }

    private void AddCategory(bool val, StatusReservation status) 
    {
        if(val) _filters.Statuses.Add(status);
        else _filters.Statuses.Remove(status);
        UpdateStatusToggles();
    }

    private void SetDateRange(bool val, DateRanges range) 
    {
        if (val) _selectedDateRange = range;
        if (range == DateRanges.AllTime)
        {
            _filters.FromDate = null;
            _filters.ToDate = null;
        }
        else if (range == DateRanges.Month) 
        {
            _filters.FromDate = DateTime.Now.AddDays(-30);
            _filters.ToDate = DateTime.Now;
        }
        else if (range == DateRanges.ThreeMonths)
        {
            _filters.FromDate = DateTime.Now.AddDays(-90);
            _filters.ToDate = DateTime.Now;
        }
        UpdateDateToggles();
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        UpdateDateToggles();
        UpdateStatusToggles();
        UIContainer.InitView(_venuesToggles, GenerateData((_data.VenueManager.GetAll())));
    }
    private List<VenueToggleView.Data> GenerateData(List<VenueModel> venues)
    {
        List<VenueToggleView.Data> list = new List<VenueToggleView.Data>();
        foreach (var v in venues)
        {
            list.Add(new VenueToggleView.Data(v, _filters.VenueId == null ? false : _filters.VenueId == v.Id));
        }
        return list;
    }
    private void UpdateDateToggles()
    {
        UIContainer.InitView(_allTime, _selectedDateRange == DateRanges.AllTime);
        UIContainer.InitView(_month, _selectedDateRange == DateRanges.Month);
        UIContainer.InitView(_3months, _selectedDateRange == DateRanges.ThreeMonths);
        UIContainer.InitView(_customDateRange, _selectedDateRange == DateRanges.Custom);

        if (_selectedDateRange == DateRanges.Custom)
        {
            _dateRange.Show();
        }
        else
        {
            _dateRange.Hide();
        }
    }
    private void UpdateStatusToggles() 
    {
        UIContainer.InitView(_includedCancelled, _filters.Statuses.Contains(StatusReservation.Cancelled));
        UIContainer.InitView(_includedBooked, _filters.Statuses.Contains(StatusReservation.Booked));
    }
}
