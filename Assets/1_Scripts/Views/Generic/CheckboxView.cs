using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckboxView : View
{
    [SerializeField] private ButtonView _action;
    [SerializeField] private Image _active;
    private bool _isOn = true;
    public override void Init<T>(T data)
    {
        if (data is bool initialState) _isOn = initialState;
        base.Init(data);
    }


    public override void UpdateUI()
    {
        base.UpdateUI();

    }
}
