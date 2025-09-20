using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderView : View
{
    [SerializeField] private Slider _slider;
    [SerializeField] private string _postfix;
    [SerializeField] private Text _min;
    [SerializeField] private Text _max;
    [SerializeField] private Text _current;
    private float _value;
    public override void Subscriptions()
    {
        base.Subscriptions();
        _slider.onValueChanged.AddListener((val) =>
        {
            _value = val;
            UpdateUI();
            TriggerAction(_value);
        });
        
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        _current.text = $"{_value:F2}{_postfix}";
    }

    public override void Init<T>(T data)
    {
        if (data is SliderData val) 
        {
            _slider.minValue = val.Min;
            _slider.maxValue = val.Max;
            _value = val.Current;
            _slider.value = _value;

            _min.text = $"{val.Min}{_postfix}";
            _max.text = $"{val.Max}{_postfix}";
        }
        base.Init(data);

    }
}
