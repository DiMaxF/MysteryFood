using System;
using System.Collections;
using System.Collections.Generic;
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
}
