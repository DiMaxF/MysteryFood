using Cysharp.Threading.Tasks;
using InfinityCode.OnlineMapsDemos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class MapScreen : AppScreen
{
    [SerializeField] private OnlineMaps _map;
    [SerializeField] private OnlineMapsMarkerManager _markers;
    [SerializeField] private UIBubblePopup _bubbles;
    [SerializeField] private Canvas _canvasPopup;
    [SerializeField] private ButtonView _userLocation;
    [SerializeField] private BaseView _loading;
    [Header("Error")]
    [SerializeField] private BaseView _error;
    [SerializeField] private ButtonView _retry;
    [SerializeField] private ButtonView _venueList;
    protected override void OnStart()
    {
        base.OnStart();
        _bubbles.OnMapClick();
        _error.Hide();
        LoadMap();
    }

    private async void LoadMap() 
    {
        _loading.Show();    
        bool isConnected = await CheckInternetAsync();
        if (isConnected)
        {
            InitPoints();
            _map.gameObject.SetActive(true);
            _canvasPopup.gameObject.SetActive(true);
            if (Data.PersonalManager.PermissionLocation)
            {
                CreateUserPoint(Data.PersonalManager.UserPosition);
                _userLocation.Show();
            }
            else _userLocation.Hide();
            _bubbles.OnMapClick();
        }
        else 
        {
            _error.Show();
        }
        _loading.Hide();
    }

    private void CreateUserPoint(GeoPoint point) 
    {
        _bubbles.CreatePoint(point);
    }

    public void GoToPoint(GeoPoint point)
    {
        _map.SetPosition(point.Longitude, point.Latitude);
        _map.zoom = 18;
    }

    private void InitPoints()
    {
        var allVenues = Data.VenueManager.GetVenuesWithCoordinates();
        _bubbles.venues = allVenues.ToArray();
    }

    private void OnDisable()
    {
        _map.gameObject.SetActive(false);
        _canvasPopup.gameObject.SetActive(false);
    }
    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(_userLocation, _ => GoToPoint(Data.PersonalManager.UserPosition));
        UIContainer.SubscribeToView<ButtonView, object>(_retry, _ => LoadMap());
        UIContainer.SubscribeToView<ButtonView, object>(_venueList, _ => 
        {
            Container.Show<HomeScreen>();
        });
    }


    public async UniTask<bool> CheckInternetAsync(CancellationToken cancellationToken = default)
    {
        if (!HasNetworkReachability())
        {
            Debug.LogWarning("Сеть недоступна (Application.internetReachability).");
            return false;
        }

        try
        {
            using (UnityWebRequest request = UnityWebRequest.Head("https://google.com"))
            {
                request.timeout = Mathf.CeilToInt(5);

                var operation = await request.SendWebRequest()
                    .WithCancellation(cancellationToken)
                    .SuppressCancellationThrow(); 

                if (operation.IsCanceled)
                {
                    Debug.LogWarning("Проверка интернета была отменена.");
                    return false;
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Интернет доступен.");
                    return true;
                }
                else
                {
                    Debug.LogWarning($"Ошибка HTTP-запроса: {request.error}");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при проверке интернета: {ex.Message}");
            return false;
        }
    }

    private bool HasNetworkReachability()
    {
        switch (Application.internetReachability)
        {
            case NetworkReachability.ReachableViaLocalAreaNetwork:
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                return true;
            case NetworkReachability.NotReachable:
            default:
                return false;
        }
    }
}