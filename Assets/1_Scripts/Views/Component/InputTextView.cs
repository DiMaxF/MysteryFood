using System;
using UnityEngine;
using UnityEngine.UI;

public class InputTextView : View
{
    [SerializeField] InputField inputField;
    [SerializeField] Image _status;
    [SerializeField] Color normalColor;
    [SerializeField] Color disableColor = Color.clear;
    [SerializeField] Color errorColor;

    public string text => inputField != null ? inputField.textComponent.text : "";
    public bool interactable
    {
        get => inputField.interactable;
        set 
        {
            if (value) DefaultColor();
            else DisableColor();
            inputField.interactable = value;
        }
    }
    public bool isValid => _status.color != errorColor;

    public override void Init<T>(T data)
    {
        if (data != null) 
        {
            inputField.text = data.ToString();
        }
        base.Init<T>(data);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        inputField.onValueChanged.AddListener((string text) =>
        {
            TriggerAction(text);
        });

    }

    public void HighlightError()
    {
        _status.color = errorColor;
    }

    public void DefaultColor()
    {
        _status.color = normalColor;
    }

    public void DisableColor() 
    {
        _status.color = disableColor;
    }
    public void UpdateColor(Color color)
    {
        inputField.colors = new ColorBlock
        {
            normalColor = color,
            highlightedColor = color,
            pressedColor = color,
            selectedColor = color,
            disabledColor = color
        };
        inputField.placeholder.color = color;
        inputField.textComponent.color = color;
    }
}
