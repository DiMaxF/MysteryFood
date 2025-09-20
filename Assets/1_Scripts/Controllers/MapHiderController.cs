using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class MapHiderController : MonoBehaviour
{
    [SerializeField] private AppContainer _navigation;
    [SerializeField] private GameObject _map;
    [SerializeField] private MapScreen _screen;
    private void Start()
    {
        _navigation.OnScreenChanged += (screen) =>
        {
            if (!(screen is MapScreen))
            {
                _map.gameObject.SetActive(false);
                _screen.CancelLoadMap();
            }
        };
    }
}
