using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DataCore : MonoBehaviour
{
    public static DataCore Instance { get; private set; }

    [SerializeField] AppData _appData = new AppData();

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
    }

    private void InitializeManagers()
    {
        VenueManager = new VenueManager(_appData);
        ReservationManager = new ReservationManager(_appData);
        PersonalManager = new PersonalManager(_appData);
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
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error loading {AppDataFileName}: {ex.Message}");
        }
    }

}