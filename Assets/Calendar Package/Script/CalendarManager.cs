using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarManager : MonoBehaviour
{
    [SerializeField] Text MonthAndYear;
    [SerializeField] private GameObject[] days;
    private int showYear;
    private int showMonth;
    [SerializeField] Button left;
    [SerializeField] Button right;
    public static int currentDateSelected;

    public event Action<DateTime?, DateTime?> OnRangeSelect;

    private void OnEnable()
    {
        left.onClick.AddListener(Left);
        right.onClick.AddListener(Right);
    }

    private void OnDisable()
    {
        left.onClick.RemoveListener(Left);
        right.onClick.RemoveListener(Right);
    }

    private void Start()
    {
        UpdateCalender(DateTime.Now.Year, DateTime.Now.Month);
    }

    private DateTime? _startDate;
    private DateTime? _endDate;

    public void UpdateCalenderWithSelectedRange(DateTime? startDate, DateTime? endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
        EnsureStartBeforeEnd();
        if (_startDate.HasValue)
        {
            UpdateCalender(_startDate.Value.Year, _startDate.Value.Month);
        }
        else
        {
            UpdateCalender(DateTime.Now.Year, DateTime.Now.Month);
        }
    }

    public void UpdateCalender(int year, int month)
    {
        showYear = year;
        showMonth = month;
        DateTime temp = new DateTime(year, month, 1);
        MonthAndYear.text = temp.ToString("MMMM") + " " + temp.ToString("yyyy");
        int startDay = GetMonthStartDay(temp.Year, temp.Month);
        int endDay = GetTotalNumberOfDays(temp.Year, temp.Month);
        int previousEndDate;

        for (int i = 0; i < days.Length; i++)
        {
            days[i].GetComponent<Day>().DayModeSet(0);
            days[i].GetComponent<Day>().dateNum = 0;
        }

        for (int w = 0; w < 6; w++)
        {
            for (int i = 0; i < 7; i++)
            {
                int currentField = (w * 7) + i;

                if (currentField < startDay || currentField - startDay >= endDay)
                {
                    days[currentField].GetComponent<Day>().DayModeSet(0);
                }
                else
                {
                    days[currentField].GetComponent<Day>().DayModeSet(2);
                }

                if (currentField >= startDay && currentField - startDay < endDay)
                {
                    var dateDay = new DateTime(showYear, showMonth, (currentField - startDay) + 1);
                    days[currentField].GetComponent<Day>().Init(dateDay, this);
                    ApplyRangeHighlight(days[currentField].GetComponent<Day>(), dateDay);
                }
                else if (currentField < startDay)
                {
                    int prevYear = temp.Month == 1 ? showYear - 1 : showYear;
                    int prevMonth = temp.Month == 1 ? 12 : temp.Month - 1;
                    previousEndDate = GetTotalNumberOfDays(prevYear, prevMonth);

                    int sub = startDay - currentField;
                    if (sub > 0 && sub < 7)
                    {
                        var dateDay = new DateTime(prevYear, prevMonth, previousEndDate - (sub - 1));
                        days[currentField].GetComponent<Day>().dateNum = previousEndDate - (sub - 1);
                        days[currentField].GetComponent<Day>().Init(dateDay, this);
                        ApplyRangeHighlight(days[currentField].GetComponent<Day>(), dateDay);
                    }
                }
                else if (currentField - startDay >= endDay)
                {
                    int nextYear = temp.Month == 12 ? showYear + 1 : showYear;
                    int nextMonth = temp.Month == 12 ? 1 : temp.Month + 1;

                    int sub = (currentField - startDay) - endDay + 1;
                    if (sub > 0 && sub < 15)
                    {
                        var dateDay = new DateTime(nextYear, nextMonth, sub);
                        days[currentField].GetComponent<Day>().Init(dateDay, this);
                        days[currentField].GetComponent<Day>().dateNum = sub;
                        ApplyRangeHighlight(days[currentField].GetComponent<Day>(), dateDay);
                    }
                }
            }
        }
    }

    private void ApplyRangeHighlight(Day dayComponent, DateTime date)
    {
        if (_startDate.HasValue && _endDate.HasValue)
        {
            if (date.Date == _startDate.Value.Date || date.Date == _endDate.Value.Date)
            {
                dayComponent.DayModeSet(1);
            }
            else if (date.Date > _startDate.Value.Date && date.Date < _endDate.Value.Date)
            {
                dayComponent.DayModeSet(3);
            }
        }
        else if (_startDate.HasValue && date.Date == _startDate.Value.Date)
        {
            dayComponent.DayModeSet(1);
        }
    }

    public void Select(DateTime date)
    {
        date = date.Date;

        if (!_startDate.HasValue)
        {
            _startDate = date;
            _endDate = null;
        }
        else if (!_endDate.HasValue)
        {
            _endDate = date;
            EnsureStartBeforeEnd();
        }
        else
        {
            if (date < _startDate.Value)
            {
                _startDate = date;
            }
            else if (date > _endDate.Value)
            {
                _endDate = date;
            }
            else
            {
                _startDate = date;
            }
            EnsureStartBeforeEnd();
        }

        if (date.Year != showYear || date.Month != showMonth)
        {
            UpdateCalender(date.Year, date.Month);
        }
        else
        {
            UpdateCalender(showYear, showMonth);
        }
        Logger.Log("OnRangeSet", "CalendarManager");
        OnRangeSelect?.Invoke(_startDate, _endDate);
    }

    private void EnsureStartBeforeEnd()
    {
        if (_startDate.HasValue && _endDate.HasValue && _startDate > _endDate)
        {
            var temp = _startDate;
            _startDate = _endDate;
            _endDate = temp;
        }
    }

    private int GetMonthStartDay(int year, int month)
    {
        DateTime temp = new DateTime(year, month, 1);
        return (int)temp.DayOfWeek;
    }

    private int GetTotalNumberOfDays(int year, int month)
    {
        return DateTime.DaysInMonth(year, month);
    }

    private void Left()
    {
        if (showMonth != 1)
        {
            UpdateCalender(showYear, showMonth - 1);
        }
        else
        {
            UpdateCalender(showYear - 1, 12);
        }
    }

    private void Right()
    {
        if (showMonth != 12)
        {
            UpdateCalender(showYear, showMonth + 1);
        }
        else
        {
            UpdateCalender(showYear + 1, 1);
        }
    }
}