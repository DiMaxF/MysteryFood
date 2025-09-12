using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : AppScreen
{
    [SerializeField] private float animationDuration = 2f;
    async void Start()
    {
        Application.targetFrameRate = 60;

        await UniTask.WaitForSeconds(animationDuration);
        Container.Show<WelcomeScreen>();
    }

    private async void CheckPermission()
    {
    }

}
