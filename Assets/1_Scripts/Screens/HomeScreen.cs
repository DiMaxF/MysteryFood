using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreen : AppScreen
{
    [SerializeField] InputTextView searchView;
    [SerializeField] ListView venues;
    [SerializeField] ButtonView hintDistance;
    [SerializeField] ButtonView addReservation;
    [SerializeField] ButtonView addVenue;
    [SerializeField] ButtonView savings;
    [SerializeField] Slider savedMoney;
    [SerializeField] Text savedMoneyValue;

    [SerializeField] private SelectVenueView _selectVenue;
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
        if (Data.PersonalManager.PermissionLocation)
        {
            hintDistance.Hide();
        }
        else
        {
            hintDistance.Show();
        }
        UIContainer.InitView(searchView, "");

        var totalSpent = Data.SavingsTrackerManager.GetTotalSpentForMonth(DateTime.Now.Year, DateTime.Now.Month);
        var savedVal = Data.SavingsTrackerManager.GetTotalSavedForMonth(DateTime.Now.Year, DateTime.Now.Month);

        // ✅ Защита от некорректных значений
        savedMoney.maxValue = Mathf.Max(1, totalSpent); // Минимум 1, чтобы избежать деления на 0
        savedMoney.value = Mathf.Max(0, savedVal); // Не показываем отрицательные значения

        Logger.Log($"Total Spent: {totalSpent}, Saved: {savedVal}", "HomeScreen");
        savedMoneyValue.text = $"{Mathf.Max(0, savedVal)}{Data.PersonalManager.Currency}";
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.RegisterView(_selectVenue);
        UIContainer.SubscribeToView<SelectVenueView, VenueModel>(_selectVenue, SelectVenueToReservation);

        UIContainer.SubscribeToView<ButtonView, object>(addReservation, _ => OnButtonAddReservation());
        UIContainer.SubscribeToView<ButtonView, object>(addVenue, _ => OnButtonAddVenue());
        UIContainer.SubscribeToView<ButtonView, object>(savings, _ => OnButtonSavings());
        UIContainer.SubscribeToView<ButtonView, object>(hintDistance, _ => RequestLocationPermission() );
        //UIContainer.SubscribeToView<ButtonView, object>(_noVenues, _ => OnButtonAddVenue());
        UIContainer.SubscribeToView<ListView, VenueModel>(venues, OnVenueAction);
        UIContainer.SubscribeToView<InputTextView, string>(searchView, OnSearchViewAction);
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        var list = _searchData == "" ? Data.VenueManager.GetAll() : Data.VenueManager.SearchVenues(_searchData, Data.VenueManager.GetAll());  
        UIContainer.InitView(venues, list);
    }

    private async void RequestLocationPermission() 
    {
        var result = await Data.RequestLocationPermission();
        if(result) hintDistance.Hide();
        else hintDistance.Show();
        //hintDistance.Hide();
    }


    private void OnButtonAddVenue()
    {
        var screen = Container.GetScreen<AddVenueScreen>();
        screen.SetModel(null);
        Container.Show(screen);
    }

    private void OnButtonAddReservation() 
    {
        UIContainer.InitView(_selectVenue, Data.VenueManager.GetAll());
        _selectVenue.Show();

    }

    private void OnButtonSavings()
    {
        Container.Show<SavingsTrackerSreen>();

    }
    private void SelectVenueToReservation(VenueModel venue)
    {
        if (venue != null)
        {
            var screen = Container.GetScreen<AddReservationScreen>();
            screen.SetVenue(venue);
            screen.Clear();
            Container.Show(screen);
        }
        else
        {

        }
        _selectVenue.Hide();
    }

    private void OnVenueAction(object obj)
    {
        if (!(obj is VenueModel model)) 
        {
            OnButtonAddVenue();
            return;
        }
        var screen = Container.GetScreen<VenueScreen>();
        screen.SetModel(model);
        Container.Show(screen);

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
            Logger.Log("Search started, waiting for delay...", "HomeScreen");
            
            // Ждем указанное время
            await UniTask.Delay(TimeSpan.FromSeconds(searchDelay), cancellationToken: _searchCancellationTokenSource.Token);
            
            // Если таймер не был отменен, выполняем поиск
            if (!_searchCancellationTokenSource.Token.IsCancellationRequested)
            {
                Logger.Log("Executing search after delay", "HomeScreen");
                UpdateViews();
            }
        }
        catch (OperationCanceledException)
        {
            // Поиск был отменен - это нормально, ничего не делаем
            Logger.Log("Search cancelled due to new input", "HomeScreen");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error in search delay: {ex.Message}", "HomeScreen");
        }
        finally
        {
            _isSearching = false;
        }
    }
    
    private void OnDestroy()
    {
        // Очищаем ресурсы при уничтожении объекта
        CancelPreviousSearch();
    }
}
