using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutUpdater : MonoBehaviour
{
    private VerticalLayoutGroup _verticalLayoutGroup;
    private HorizontalLayoutGroup _horizontalLayoutGroup;
    [SerializeField] private float _animationDuration = 1f;
    private Tween _spacingTween;

    private void OnEnable()
    {
        if (_verticalLayoutGroup == null) _verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        if (_horizontalLayoutGroup == null) _horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
        UpdateSpacing();
    }

    public void UpdateSpacing()
    {

        _spacingTween?.Kill();
        if (_verticalLayoutGroup != null) 
        {
            _spacingTween = DOTween.To(
                () => _verticalLayoutGroup.spacing,
                value => _verticalLayoutGroup.spacing = value,
                _verticalLayoutGroup.spacing++,
                _animationDuration
            ).SetEase(Ease.Linear);
        }
        if (_horizontalLayoutGroup != null)
        {
            _spacingTween = DOTween.To(
                () => _horizontalLayoutGroup.spacing,
                value => _horizontalLayoutGroup.spacing = value,
                _horizontalLayoutGroup.spacing++,
                _animationDuration
            ).SetEase(Ease.Linear);
        }
    }
    private void OnDisable()
    {
        _spacingTween?.Kill();
    }
}
