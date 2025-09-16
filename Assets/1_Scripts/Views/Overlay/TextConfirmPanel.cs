using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextConfirmPanel : View
{
    [SerializeField] private ConfirmPanel _confirmPanel;
    [SerializeField] private InputTextView _type;
    [SerializeField] private string _target;


    public override void UpdateUI()
    {
        base.UpdateUI();
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<InputTextView, string>(_type, ValidateInoutText);
        UIContainer.SubscribeToView<ConfirmPanel, bool>(_confirmPanel, TriggerAction);
    }

    private void ValidateInoutText(string text) 
    {
        if (text == _target)
        {
            _type.DefaultColor();
            _confirmPanel.SetActive(true);
        }
        else 
        {
            _type.HighlightError();
            _confirmPanel.SetActive(false);
        }
    }

    public override void Init<T>(T data)
    {
        UIContainer.RegisterView(_confirmPanel);
        UIContainer.RegisterView(_type);
        UIContainer.InitView(_type, "");
        UIContainer.InitView(_confirmPanel, "Type DELETE to confirm");
        base.Init(data);
    }
}
