using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusView : View
{
    [SerializeField] private Text _label;
    [SerializeField] private Image _indicator;
    private StatusReservation _status;  

    
    public override void UpdateUI()
    {
        base.UpdateUI();
        _label.text = _status.ToString();
        _indicator.color = GetColor();
    }

    public override void Init<T>(T data)
    {
        if (data is StatusReservation status) _status = status;
        base.Init(data);
    }

    private Color GetColor() 
    {
        return (_status) switch
        {
            StatusReservation.Booked => new Color(127f / 255f, 248f / 255f, 3f / 255f),
            StatusReservation.PickedUp => new Color(5f/255f, 141f/255f, 252f/255f),
            StatusReservation.Cancelled => new Color(255f, 0f, 0f),
        };
    }
}
