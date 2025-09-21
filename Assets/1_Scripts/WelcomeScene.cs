using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeScene : MonoBehaviour
{
    [SerializeField] private GameObject  _loading;
    [SerializeField] private Button _goToMain;

    private async void Start()
    {
        _loading.SetActive(true);
        await UniTask.WaitForSeconds(2);
        _loading.SetActive(false);
        _goToMain.onClick.AddListener(() => {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
        });
    }
}
