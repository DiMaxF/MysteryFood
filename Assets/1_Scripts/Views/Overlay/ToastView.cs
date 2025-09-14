using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToastView : View
{
    [SerializeField] private Text _text;
    [SerializeField] private float _timeShow = 1.75f;
    private string _message;

    private bool _show;
    public override void UpdateUI()
    {
        base.UpdateUI();
        _text.text = _message;
    }

    public override void Init<T>(T data)
    {
        if (data is string message) _message = message;
        base.Init(data);
    }

    public async override void Show()
    {
        if (_show) return;
        _show = true;
        base.Show();
        await UniTask.WaitForSeconds(_timeShow);
        Hide();
    }

    public override void Hide()
    {
        base.Hide();
        _show = false;
    }

}
