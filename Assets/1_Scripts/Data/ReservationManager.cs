using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReservationManager : IDataManager
{
    private readonly AppData _appData;
    private int _index
    {
        set => PlayerPrefs.SetInt("ReservationIndex", value);
        get => PlayerPrefs.GetInt("ReservationIndex", 0);
    }
    public ReservationManager(AppData appData)
    {
        _appData = appData ?? throw new ArgumentNullException(nameof(appData));
    }

    public void Clear()
    {
        
    }

    public void Add(ReservationModel model)
    {
        model.Id = _index;
        _index++;
        _appData.Reservations.Add(model);
    }

    public List<ReservationModel> GetSorted(StatusReservation status) 
    {
        return GetAll().Where(r => r.Status == status).ToList();
    }

    public List<ReservationModel> GetAll() 
    {
        return _appData.Reservations;
    }


    public bool Update(ReservationModel updated)
    {
        if (updated == null)
        {
            Debug.LogError("Updated venue is null");
            return false;
        }
        var existing = GetById(updated.Id);

        if (existing == null) return false;

        existing.OriginalPrice = updated.OriginalPrice;
        existing.DiscountedPrice = updated.DiscountedPrice;
        existing.StartTime = updated.StartTime;
        existing.EndTime = updated.EndTime;
        existing.Notes = updated.Notes;
        existing.QrPath = updated.QrPath;
        existing.Notification = updated.Notification;
        existing.Status = updated.Status;

        Logger.Log($"Reservation with Id {updated.Id} updated successfully", "ReservationManager");
        return true;
    }

    public ReservationModel GetById(int id)
    {
        var existing = _appData.Reservations.FirstOrDefault(v => v.Id == id);
        if (existing == null)
        {
            Logger.LogWarning($"Venue with Id {id} not found", "ReservationManager");
            return null;
        }
        return existing;
    }
}
