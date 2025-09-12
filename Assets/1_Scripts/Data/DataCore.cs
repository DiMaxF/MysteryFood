using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DataCore : MonoBehaviour
{
    public static DataCore Instance { get; private set; }

    [SerializeField] AppData _appData = new AppData();

    private EventManager _eventManager;
    public VenueManager VenueManager 
    {
        private set;
        get;
    }
    private AnalyticsManager _analyticsManager;
    private PersonalManager _personalManager;

   
    public EventManager Events => _eventManager;
    public AnalyticsManager Analytics => _analyticsManager;
    public PersonalManager Personal => _personalManager;

    private const string AppDataFileName = "appData.json";

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

        _eventManager = new EventManager(_appData);
        _personalManager = new PersonalManager(_appData);
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
        FileManager.WriteToFile(AppDataFileName, json);
        Logger.Log("GlobalData saved successfully");
    }
    private void LoadData()
    {
        if (!FileManager.FileExists(AppDataFileName))
        {
            Logger.Log($"{AppDataFileName} not found, starting with default data");
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