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
using UnityEngine.UIElements;

public class MapScreen : AppScreen
{
    [SerializeField] private OnlineMaps _map;
    [SerializeField] private OnlineMapsMarkerManager _markers;
    [SerializeField] private UIBubblePopup _bubbles;
    [SerializeField] private Canvas _canvasPopup;
    [SerializeField] private ButtonView _userLocation;
    [SerializeField] private BaseView _loading;
    [SerializeField] InputTextView _searchView;
    [SerializeField] LocationManager _locationManager;

    [Header("Action")]
    [SerializeField] private ButtonView _reservation;
    [SerializeField] private ButtonView _openVenue;
    [SerializeField] private ButtonView _openDirection;
    [SerializeField] private ButtonView _openFilters;
    [Header("Error")]
    [SerializeField] private BaseView _error;
    [SerializeField] private ButtonView _retry;
    [SerializeField] private ButtonView _venueList;
    [SerializeField] private ButtonView _closeMap;
    [Header("Overlay")]
    [SerializeField] private MapFiltersView _filters;
    private VenueModel _selectedVenue;
    private FilterOptions _filtersOptions = new FilterOptions();
    private CancellationTokenSource _loadMapCts;
    private string _searchData = "";
    
    // Поиск с кулдауном
    private CancellationTokenSource _searchCancellationTokenSource;
    [SerializeField] private float searchDelay = 0.3f; // Настраиваемая задержка поиска
    private bool _isSearching = false; // Флаг для отслеживания состояния поиска
    
    /// <summary>
    /// Возвращает true, если поиск в процессе выполнения
    /// </summary>
    public bool IsSearching => _isSearching;

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
                isFirstUpdate = false; 
                Logger.Log("MAP", "Map updated, markers created");
            }
        };

        _bubbles.OnMapClick();
        _error.Hide();
        LoadMap();
        UIContainer.InitView(_searchView, "");


    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        UIContainer.InitView(_filters, _filtersOptions);
    }
    private async void LoadMap(CancellationToken cancellationToken = default)
    {
        _loading.Show();
        try
        {
            bool isConnected = await CheckInternetAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested(); 
            if (isConnected)
            {
                DrawVenuesOnMap();
                cancellationToken.ThrowIfCancellationRequested(); 
                _map.gameObject.SetActive(true);
                _canvasPopup.gameObject.SetActive(true);
                CreateUserPoint();
                _bubbles.OnMapClick();
            }
            else
            {
                _error.Show();
            }
        }
        catch (OperationCanceledException)
        {
            Logger.Log("MAP", "LoadMap was cancelled");
            _error.Show();
        }
        catch (Exception ex)
        {
            Logger.LogError($"LoadMap failed: {ex.Message}");
            _error.Show();
        }
        finally
        {
            _loading.Hide();
        }
    }
    private void LoadMap()
    {
        CancelLoadMap(); 
        _loadMapCts = new CancellationTokenSource();
        LoadMap(_loadMapCts.Token);
    }
    public void CancelLoadMap()
    {
        if (_loadMapCts != null)
        {
            _loadMapCts.Cancel();
            _loadMapCts.Dispose();
            _loadMapCts = null;
            Logger.Log("MAP", "LoadMap cancellation requested");
        }
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
        if (point == null) return;
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
        CancelLoadMap();
    }
    
    private void OnDestroy()
    {
        // Очищаем ресурсы при уничтожении объекта
        CancelPreviousSearch();
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        _bubbles.OnVenueSelected += (venue) =>
        {
            _selectedVenue = venue;
        };
        UIContainer.SubscribeToView<MapFiltersView, FilterOptions>(_filters, ApplyFilters);
        UIContainer.SubscribeToView(_closeMap, (object _) => _bubbles.OnMapClick());

        UIContainer.SubscribeToView<ButtonView, object>(_userLocation, _ => OnUserPosition());
        UIContainer.SubscribeToView<ButtonView, object>(_retry, _ => LoadMap());
        UIContainer.SubscribeToView(_reservation, (object _) => AddReservation());
        UIContainer.SubscribeToView(_openVenue, (object _) => OpenVenue());
        UIContainer.SubscribeToView(_openDirection, (object _) => OpenDirections());
        UIContainer.SubscribeToView(_openFilters, (object _) => OpenFilters());
        UIContainer.SubscribeToView<ButtonView, object>(_venueList, _ => 
        {
            Container.Show<HomeScreen>();
        });
        UIContainer.SubscribeToView<InputTextView, string>(_searchView, OnSearchViewAction);
    }
    private async void OnUserPosition() 
    {
        var point = await _locationManager.GetLocationAsync();
        Data.PersonalManager.UserPosition = point;
        GoToPoint(Data.PersonalManager.UserPosition);
        _bubbles.UpdateMarkers(true);
    }
    private void OpenFilters() 
    {
        _filters.Show();
    }

    private void ApplyFilters(FilterOptions filters) 
    {
        _filtersOptions = filters;
        DrawVenuesOnMap();
        CreateUserPoint();
        _filters.Hide();
        _bubbles.UpdateMarkers(true);
    }

    private void OnSearchViewAction(string val)
    {
        _searchData = val;
        
        // Отменяем предыдущий поиск, если он еще не выполнился
        CancelPreviousSearch();
        
        // Запускаем новый поиск с кулдауном
        StartSearchWithDelay();
    }
    
    private void CancelPreviousSearch()
    {
        if (_searchCancellationTokenSource != null)
        {
            _searchCancellationTokenSource.Cancel();
            _searchCancellationTokenSource.Dispose();
        }
        _searchCancellationTokenSource = new CancellationTokenSource();
    }
    
    private async void StartSearchWithDelay()
    {
        try
        {
            _isSearching = true;
            Logger.Log("Search started, waiting for delay...", "MapScreen");
            
            // Ждем указанное время
            await UniTask.Delay(TimeSpan.FromSeconds(searchDelay), cancellationToken: _searchCancellationTokenSource.Token);
            
            // Если таймер не был отменен, выполняем поиск
            if (!_searchCancellationTokenSource.Token.IsCancellationRequested)
            {
                Logger.Log("Executing search after delay", "MapScreen");
                DrawVenuesOnMap();
            }
        }
        catch (OperationCanceledException)
        {
            // Поиск был отменен - это нормально, ничего не делаем
            Logger.Log("Search cancelled due to new input", "MapScreen");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error in search delay: {ex.Message}", "MapScreen");
        }
        finally
        {
            _isSearching = false;
        }
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
        screen.Clear();
        screen.SetVenue(_selectedVenue);
        Container.Show(screen);
    }

    private async void DrawVenuesOnMap() 
    {
        if (_filtersOptions != new FilterOptions())
        {
            var list = _searchData == "" ? Data.VenueManager.GetFilteredVenues(_filtersOptions, Data.PersonalManager) : Data.VenueManager.SearchAdresses(_searchData, Data.VenueManager.GetFilteredVenues(_filtersOptions, Data.PersonalManager));
            InitPoints(list);
        }
        else 
        {
            var list = _searchData == "" ? Data.VenueManager.GetVenuesWithCoordinates() : Data.VenueManager.SearchAdresses(_searchData, Data.VenueManager.GetVenuesWithCoordinates());
            InitPoints(list);
        }
        await UniTask.WaitForSeconds(0.3f);
        _bubbles.UpdateMarkers(true);
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
            Logger.LogWarning("���� ���������� (Application.internetReachability).");
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
                    Logger.LogWarning("�������� ��������� ���� ��������.");
                    return false;
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Logger.Log("�������� ��������.");
                    return true;
                }
                else
                {
                    Logger.LogWarning($"������ HTTP-�������: {request.error}");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"������ ��� �������� ���������: {ex.Message}");
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