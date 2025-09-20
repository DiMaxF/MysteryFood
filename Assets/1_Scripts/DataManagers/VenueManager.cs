using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VenueManager : IDataManager
{
    private readonly AppData _appData;

    private int _index 
    {
        set => PlayerPrefs.SetInt("VenueIndex", value);
        get => PlayerPrefs.GetInt("VenueIndex", 0);
    }

    public VenueManager(AppData appData)
    {
        _appData = appData ?? throw new ArgumentNullException(nameof(appData));
    }

    public void Clear()
    {
        _appData.Venues = new List<VenueModel>();
    }

    public List<VenueModel> GetVenuesWithCoordinates() 
    {
        return GetAll().Where(v => v.Location.Longitude != 0).ToList();
    }
    public List<VenueModel> GetAll() => _appData.Venues;

    public List<VenueModel> SearchVenues(string searchData, List<VenueModel> list) 
    {
        var result = list.Where(v => v.Id.ToString().Contains(searchData) || v.Name.Contains(searchData));
        return result.ToList(); 
    }

    public List<VenueModel> SearchAdresses(string searchData, List<VenueModel> list) 
    {
        return list.Where(v =>v.Name.Contains(searchData) || v.Location.Address.Contains(searchData)).ToList();
    }

    public void AddVenue(VenueModel venue) 
    {
        venue.Id = _index;
        _index++;
        _appData.Venues.Add(venue); 
    }

    public bool UpdateVenue(VenueModel updatedVenue)
    {
        if (updatedVenue == null)
        {
            Debug.LogError("Updated venue is null");
            return false;
        }
        var existingVenue = GetById(updatedVenue.Id);

        if (existingVenue == null) return false;

        existingVenue.Name = updatedVenue.Name;
        existingVenue.Location = updatedVenue.Location;
        existingVenue.Phone = updatedVenue.Phone;
        existingVenue.Price = updatedVenue.Price;
        existingVenue.StartTime = updatedVenue.StartTime;
        existingVenue.EndTime = updatedVenue.EndTime;
        existingVenue.Description = updatedVenue.Description;
        existingVenue.IngredientsAllergens = updatedVenue.IngredientsAllergens;
        existingVenue.ImagePath = updatedVenue.ImagePath;

        Debug.Log($"Venue with Id {updatedVenue.Id} updated successfully");
        return true;
    }

    public VenueModel GetById(int id) 
    {
        var existingVenue = _appData.Venues.FirstOrDefault(v => v.Id == id);
        if (existingVenue == null)
        {
            Debug.LogWarning($"Venue with Id {id} not found");
            return null;
        }
        return existingVenue;
    }


    public void UpdateCurrency(Currency currency)
    {
        foreach (var r in GetAll())
        {
            r.Price.Amount = r.Price.ConvertTo(currency);
            r.Price.Currency = currency;
        }
    }

    public List<VenueModel> GetFilteredVenues(FilterOptions filters, PersonalManager personalManager)
    {
        var venues = GetVenuesWithCoordinates();

        if (filters != null)
        {
            if (filters.MaxDistanceKm.HasValue && personalManager.UserPosition != null)
            {
                venues = venues.Where(v => personalManager.CalculateDistance(v.Location) <= filters.MaxDistanceKm.Value).ToList();
            }

            if (filters.MinPrice.HasValue)
            {
                venues = venues.Where(v => v.Price.Amount >= filters.MinPrice.Value).ToList();
            }

            if (filters.MaxPrice.HasValue)
            {
                venues = venues.Where(v => v.Price.Amount <= filters.MaxPrice.Value).ToList();
            }

            if (filters.OnlyOpenNow)
            {
                var now = DateTime.Now.TimeOfDay;
                venues = venues.Where(v =>
                {
                    var start = TimeSpan.Parse(v.StartTime);
                    var end = TimeSpan.Parse(v.EndTime);
                    return now >= start && now <= end;
                }).ToList();
            }
        }

        return venues;
    }
}
