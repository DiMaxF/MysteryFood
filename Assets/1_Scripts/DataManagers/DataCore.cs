using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class DataCore : MonoBehaviour
{
    public static DataCore Instance { get; private set; }

    [SerializeField] AppData _appData = new AppData();
    [SerializeField] LocationManager _location;

    public VenueManager VenueManager 
    {
        private set;
        get;
    }
    public ReservationManager ReservationManager
    {
        private set;
        get;
    }
    public PersonalManager PersonalManager
    {
        private set;
        get;
    }
    public SavingsTrackerManager SavingsTrackerManager
    {
        private set;
        get;
    }
    public NotificationManager NotificationManager 
    { 
        private set;
        get; 
    }

    private bool FirstEnter 
    {
        get => PlayerPrefs.GetInt("FirstEnter") == 1;
        set => PlayerPrefs.SetInt("FirstEnter", value ? 1 : 0); 
    }

    private const string AppDataFileName = "dsdsd.json";

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManagers();
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
        if (FirstEnter) 
        {
            RequestLocationPermission();
            RequestNotificationPermission();
        }
        if (PersonalManager.PermissionLocation) 
        {
            UpdateUserLocation();
        }
        FirstEnter = false;
    }
    public async void RequestNotificationPermission()
    {
        bool granted = await NotificationManager.RequestNotificationPermissionAsync();
        PersonalManager.PermissionNotification = granted;
        if (granted)
        {
            NotificationManager.RebuildNotificationQueue(); 
        }
    }
    public async void RequestLocationPermission() 
    {
        if (!await _location.CheckAndRequestPermissionAsync())
        {
            PersonalManager.PermissionLocation = false;
        }
        else 
        {
            UpdateUserLocation();
            PersonalManager.PermissionLocation = true;
        }
        
    }

    private async void UpdateUserLocation() 
    {
        while (true) 
        {
            await UniTask.Delay(TimeSpan.FromMinutes(1));   
            PersonalManager.UserPosition = await _location.GetLocationAsync();
        } 
    }

    private void InitializeManagers()
    {
        VenueManager = new VenueManager(_appData);
        PersonalManager = new PersonalManager(_appData);
        SavingsTrackerManager = new SavingsTrackerManager(_appData);
        NotificationManager = new NotificationManager(_appData);
        ReservationManager = new ReservationManager(_appData, NotificationManager);

    }

    public void ClearData() 
    {
        VenueManager.Clear();
        ReservationManager.Clear();
        PersonalManager.Clear();
    }

    public void SetDefault()
    {
        PersonalManager.Clear();
    }

    /// <summary>
    /// Discards changes and reloads data.
    /// </summary>
    public void DiscardChanges()
    {
        LoadData();
    }

    /// <summary>
    /// Saves data to file.
    /// </summary>
    public void SaveData()
    {
        string json = JsonUtility.ToJson(_appData, true);
        FileManager.WriteToFile(AppDataFileName, json).Forget();
        Logger.Log("Data saved successfully", "DataCore");
    }
    private void LoadData()
    {
        if (!FileManager.FileExists(AppDataFileName))
        {
            Logger.Log($"{AppDataFileName} not found, starting with default data", "DataCore");
            return;
        }

        string json = FileManager.ReadFile(AppDataFileName);
        try
        {
            AppData loadedData = JsonUtility.FromJson<AppData>(json);
            if (loadedData != null)
            {
                _appData = loadedData;
                InitializeManagers();
                NotificationManager.RebuildNotificationQueue();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error loading {AppDataFileName}: {ex.Message}");
        }
    }

}