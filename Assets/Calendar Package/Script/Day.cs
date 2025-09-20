using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Day : MonoBehaviour
{
    public enum DayMode
    {
        Disabled = 0, 
        Current = 1,
        Normal = 2,
        Highlighted = 3
    }

    [SerializeField] private DayMode dayMode; 
    private Button button; 
    [SerializeField] private GameObject currentDateIndicator; 
    [SerializeField] private GameObject hightlited; 
    [SerializeField] private Text dateText;
    public int dateNum; 
    public event Action<DateTime> OnSelect;



    void SetUpUI()
    {
        dateText.text = _date.Day.ToString();
        if(button == null) button = GetComponent<Button>();
        switch (dayMode)
        {
            case DayMode.Disabled:
                dateText.color = Color.gray;
                button.interactable = false;
                currentDateIndicator.SetActive(false);
                hightlited.SetActive(false);
                break;
            case DayMode.Current:
                dateText.color = Color.black;
                button.interactable = true;
                currentDateIndicator.SetActive(true);
                hightlited.SetActive(false);
                break;
            case DayMode.Normal:
                dateText.color = Color.white;
                button.interactable = true;
                currentDateIndicator.SetActive(false);
                hightlited.SetActive(false);
                break;
            case DayMode.Highlighted:
                dateText.color = Color.white;
                button.interactable = true;
                currentDateIndicator.SetActive(false);
                hightlited.SetActive(true);
                break;
        }
    }

    public void DayModeSet(int index)
    {
        dayMode = (DayMode)index;

        SetUpUI();
    }

    DateTime _date;
    CalendarManager _manager;
    public void Init(DateTime date, CalendarManager manager) 
    {
        _date = date;
        _manager = manager;
        SetUpUI(); 
    }


    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => _manager.Select(_date));
    }
}
