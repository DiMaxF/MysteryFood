using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualCounterView : View
{
    [SerializeField] private ButtonView _plus;
    [SerializeField] private ButtonView _minus;
    [SerializeField] private Text _value;

    private int _counter;

    public override void Init<T>(T data)
    {
        if (data is int val) 
        {
            _counter = val;
            UIContainer.RegisterView(_plus);
            UIContainer.RegisterView(_minus);
        }
        base.Init(data);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView(_plus, (object _) => AddCount(1));
        UIContainer.SubscribeToView(_minus, (object _) => AddCount(-1));
    }
    public override void UpdateUI()
    {
        base.UpdateUI();
        _value.text = _counter.ToString();  
        _minus.canvasGroup.enabled = _counter > 0;
    }

    private void AddCount(int value) 
    {
        _counter += value;
        UpdateUI();
        TriggerAction(_counter);   
    }


        
}
