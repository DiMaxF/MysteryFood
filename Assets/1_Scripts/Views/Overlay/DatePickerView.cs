using DG.Tweening;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class DatePickerView : View
{
    [SerializeField] CalendarManager calendar;

    private CanvasGroup canvasGroup;
    (string, string) _date;
    public override void Init<T>(T data)
    {

        base.Init(data);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        calendar.OnRangeSelect += (start, end) => _date = (start?.ToString(DateTimeUtils.Format), end?.ToString(DateTimeUtils.Format));
    }


    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }
}