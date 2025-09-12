using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextColorAnimation : MonoBehaviour, IViewAnimation
{
    [SerializeField] AnimationConfig config;
    [SerializeField] Color show;
    [SerializeField] Color hide;
    public int Order => 0;
    public bool IsParallel => true;

    [SerializeField] private Text _text;


    public Tween AnimateHide()
    {
        _text.color = show;
        return _text.DOColor(hide, config.Duration).SetEase(config.Ease);
    }

    public Tween AnimateShow()
    {
        _text.color = hide;
        return _text.DOColor(show, config.Duration).SetEase(config.Ease);
    }
}
