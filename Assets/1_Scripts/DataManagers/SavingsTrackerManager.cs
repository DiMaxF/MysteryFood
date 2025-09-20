using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class SavingsTrackerManager : IDataManager
{
    private readonly AppData _appData;
    public SavingsTrackerManager(AppData appData)
    {
        _appData = appData ?? throw new ArgumentNullException(nameof(appData));
    }

    private FilterOptions _filters;

    public void ApplyFilters(FilterOptions filters)
    {
        _filters = filters;
    }

    public void Clear()
    {
        _filters = null;
    }

    private List<ReservationModel> GetFilteredReservations()
    {
        var reservations = _appData.Reservations;

        if (_filters != null)
        {
            if (_filters.FromDate.HasValue)
            {
                reservations = reservations.Where(r =>
                    DateTime.ParseExact(r.CreatedAt, DateTimeUtils.Full, CultureInfo.InvariantCulture) >= _filters.FromDate.Value)
                    .ToList();
            }

            if (_filters.ToDate.HasValue)
            {
                reservations = reservations.Where(r =>
                    DateTime.ParseExact(r.CreatedAt, DateTimeUtils.Full, CultureInfo.InvariantCulture) <= _filters.ToDate.Value)
                    .ToList();
            }

            if (_filters.VenueId.HasValue)
            {
                reservations = reservations.Where(r => r.VenueId == _filters.VenueId.Value).ToList();
            }

            if (_filters.Statuses != null && _filters.Statuses.Any())
            {
                reservations = reservations.Where(r => _filters.Statuses.Contains(r.Status)).ToList();
            }
        }

        return reservations;
    }

    public float GetTotalSaved()
    {
        var filtered = GetFilteredReservations();
        return filtered.Sum(r => (r.OriginalPrice.Amount - r.DiscountedPrice.Amount) * r.Quantity);
    }

    public int GetBagsCollected()
    {
        var filtered = GetFilteredReservations();//.Where(r => r.Status == StatusReservation.PickedUp);
        return filtered.Sum(r => r.Quantity);
    }

    public float GetCO2EAvoided()
    {
        return _appData.CO2E * GetBagsCollected();
    }

    public float GetFoodWastePrevented()
    {
        return _appData.WasteBag * GetBagsCollected();
    }

    public ChartData GetMonthlySavingsChartData()
    {
        var filtered = GetFilteredReservations();//.Where(r => r.Status == StatusReservation.PickedUp);
        var monthlySavings = new Dictionary<string, float>();

        foreach (var r in filtered)
        {
            DateTime dt = DateTime.ParseExact(r.CreatedAt, DateTimeUtils.Full, CultureInfo.InvariantCulture);
            string key = dt.ToString("MMM yyyy", CultureInfo.InvariantCulture);
            float saving = (r.OriginalPrice.Amount - r.DiscountedPrice.Amount) * r.Quantity;
            if (monthlySavings.ContainsKey(key))
            {
                monthlySavings[key] += saving;
            }
            else
            {
                monthlySavings[key] = saving;
            }
        }

        var sortedKeys = monthlySavings.Keys
            .Select(k => new { Key = k, Date = DateTime.ParseExact(k, "MMM yyyy", CultureInfo.InvariantCulture) })
            .OrderBy(x => x.Date)
            .Select(x => x.Key)
            .ToList();

        var sortedSavings = new Dictionary<string, float>();
        foreach (var key in sortedKeys)
        {
            sortedSavings[key] = monthlySavings[key];
        }

        return new ChartData("Monthly Savings", sortedSavings, null);
    }

    public ChartData GetBagsOverTimeChartData()
    {
        var filtered = GetFilteredReservations();//.Where(r => r.Status == StatusReservation.PickedUp);
        var monthlyBags = new Dictionary<string, int>();

        foreach (var r in filtered)
        {
            DateTime dt = DateTime.ParseExact(r.CreatedAt, DateTimeUtils.Full, CultureInfo.InvariantCulture);
            string key = dt.ToString("MMM yyyy", CultureInfo.InvariantCulture);
            if (monthlyBags.ContainsKey(key))
            {
                monthlyBags[key] += r.Quantity;
            }
            else
            {
                monthlyBags[key] = r.Quantity;
            }
        }

        var sortedKeys = monthlyBags.Keys
            .Select(k => new { Key = k, Date = DateTime.ParseExact(k, "MMM yyyy", CultureInfo.InvariantCulture) })
            .OrderBy(x => x.Date)
            .Select(x => x.Key)
            .ToList();

        var sortedBags = new Dictionary<string, float>();
        foreach (var key in sortedKeys)
        {
            sortedBags[key] = monthlyBags[key];
        }

        return new ChartData("Bags Over Time", sortedBags, null);
    }

    public List<ReservationModel> GetRecentActivity(int count = 10)
    {
        return GetFilteredReservations()
            .OrderByDescending(r => DateTime.ParseExact(r.CreatedAt, DateTimeUtils.Full, CultureInfo.InvariantCulture))
            .Take(count)
            .ToList();
    }
}