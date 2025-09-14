using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPanel : View
{
    [SerializeField] private Text _text;
    [SerializeField] private ButtonView _cancel;
    [SerializeField] private ButtonView _confirm;
    private string _message;

    public override void UpdateUI()
    {
        base.UpdateUI();
        _text.text = _message;
    }

    public override void Init<T>(T data)
    {
        if (data is string message) _message = message;
        UIContainer.RegisterView(_cancel);
        UIContainer.RegisterView(_confirm);
        base.Init(data);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView(_cancel, (object _) => TriggerAction(false));
        UIContainer.SubscribeToView(_confirm, (object _) => TriggerAction(true));
    }
}
