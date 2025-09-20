using Cysharp.Threading.Tasks;
using InfinityCode.OnlineMapsDemos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using UnityEditor;
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
    [Header("Action")]
    [SerializeField] private ButtonView _reservation;
    [SerializeField] private ButtonView _openVenue;
    [SerializeField] private ButtonView _openDirection;
    [SerializeField] private ButtonView _openFilters;
    [Header("Error")]
    [SerializeField] private BaseView _error;
    [SerializeField] private ButtonView _retry;
    [SerializeField] private ButtonView _venueList;

    [Header("Overlay")]
    [SerializeField] private MapFiltersView _filters;
    private VenueModel _selectedVenue;
    private FilterOptions _filtersOptions = new FilterOptions();
    protected override void OnStart()
    {
        base.OnStart();
        UIContainer.RegisterView(_filters);
        bool isFirstUpdate = true;
        _map.OnMapUpdated += () =>
        {
            if (isFirstUpdate)
            {
                _bubbles.UpdateMarkers(true);
                isFirstUpdate = false; // Вызываем только один раз
                Logger.Log("MAP", "Map updated, markers created");
            }
        };
        Container.OnScreenChanged += (screen) =>
        {
            if (!(screen is MapScreen))
            {
                _map.gameObject.SetActive(false);
                _canvasPopup.gameObject.SetActive(false);
            }
        };  
        _bubbles.OnMapClick();
        _error.Hide();
        LoadMap();
        
        
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        UIContainer.InitView(_filters, _filtersOptions);
    }

    private async void LoadMap() 
    {
        _loading.Show();    
        bool isConnected = await CheckInternetAsync();
        if (isConnected)
        {
            InitPoints(Data.VenueManager.GetVenuesWithCoordinates());
            _map.gameObject.SetActive(true);
            _canvasPopup.gameObject.SetActive(true);
            CreateUserPoint();
            _bubbles.OnMapClick();
        }
        else 
        {
            _error.Show();
        }
        _loading.Hide();
    }

    private void CreateUserPoint() 
    {
        if (Data.PersonalManager.PermissionLocation)
        {
            _bubbles.CreatePoint(Data.PersonalManager.UserPosition);
            _userLocation.Show();
        }
        else _userLocation.Hide();
        
    }

    public void GoToPoint(GeoPoint point)
    {
        _map.SetPosition(point.Longitude, point.Latitude);
        _map.zoom = 18;
    }

    private void InitPoints(List<VenueModel> points)
    {
        OnlineMapsMarkerManager.RemoveAllItems();
        
        _bubbles.venues = points.ToArray();
    }

    private void OnDisable()
    {
        _map.gameObject.SetActive(false);
        _canvasPopup.gameObject.SetActive(false);
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        _bubbles.OnVenueSelected += (venue) =>
        {
            _selectedVenue = venue;
        };
        UIContainer.SubscribeToView<MapFiltersView, FilterOptions>(_filters, ApplyFilters);

        UIContainer.SubscribeToView<ButtonView, object>(_userLocation, _ => GoToPoint(Data.PersonalManager.UserPosition));
        UIContainer.SubscribeToView<ButtonView, object>(_retry, _ => LoadMap());
        UIContainer.SubscribeToView(_reservation, (object _) => AddReservation());
        UIContainer.SubscribeToView(_openVenue, (object _) => OpenVenue());
        UIContainer.SubscribeToView(_openDirection, (object _) => OpenDirections());
        UIContainer.SubscribeToView(_openFilters, (object _) => OpenFilters());
        UIContainer.SubscribeToView<ButtonView, object>(_venueList, _ => 
        {
            Container.Show<HomeScreen>();
        });
    }

    private void OpenFilters() 
    {
        _filters.Show();
    }

    private void ApplyFilters(FilterOptions filters) 
    {
        InitPoints(Data.VenueManager.GetFilteredVenues(filters, Data.PersonalManager));
        CreateUserPoint();
        _filters.Hide();
        _bubbles.UpdateMarkers(true);
    }
    private void OpenVenue() 
    {
        var screen = Container.GetScreen<VenueScreen>();
        screen.SetModel(_selectedVenue);
        Container.Show(screen);
    }

    private void AddReservation()
    {
        var screen = Container.GetScreen<AddReservationScreen>();
        screen.SetVenue(_selectedVenue);
        Container.Show(screen);
    }

    private void OpenDirections() 
    {
        var latitude = _selectedVenue.Location.Latitude;
        var longitude = _selectedVenue.Location.Longitude;
#if UNITY_ANDROID
        string url = string.Format("https://www.google.com/maps/search/?api=1&query={0},{1}", latitude, longitude);
#elif UNITY_IOS
        string url = string.Format("maps://maps.apple.com/?q={0},{1}", latitude, longitude);
#else
        string url = string.Format("https://www.google.com/maps/search/?api=1&query={0},{1}", latitude, longitude);
#endif

        Application.OpenURL(url);
    }

    public async UniTask<bool> CheckInternetAsync(CancellationToken cancellationToken = default)
    {
        if (!HasNetworkReachability())
        {
            Logger.LogWarning("Сеть недоступна (Application.internetReachability).");
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
                    Logger.LogWarning("Проверка интернета была отменена.");
                    return false;
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Logger.Log("Интернет доступен.");
                    return true;
                }
                else
                {
                    Logger.LogWarning($"Ошибка HTTP-запроса: {request.error}");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Ошибка при проверке интернета: {ex.Message}");
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