using Cysharp.Threading.Tasks;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AsyncImageView : View
{
    [SerializeField] private RawImage _image;
    [SerializeField] private GameObject _loading;
    [SerializeField] private GameObject _placeholder;
    private string _imagePath;
    private AspectRatioFitter aspectRatioFitter;
    public bool widthFill; 

    public override void Init<T>(T data)
    {
        if (data is string path) _imagePath = path;
        InitAspectRatio();
        base.Init(data);
    }

    private void InitAspectRatio() 
    {
        if (aspectRatioFitter == null)
        {
            aspectRatioFitter = _image.GetComponent<AspectRatioFitter>();
            if (aspectRatioFitter == null) aspectRatioFitter = _image.gameObject.AddComponent<AspectRatioFitter>();
        }
    }

    public override void UpdateUI()
    {
        Loading(true);
        if (string.IsNullOrEmpty(_imagePath))
        {
            TriggerError();
            return;
        }
        if (_loading != null) _loading.SetActive(true); 

        if (IsLocalFilePath(_imagePath))
        {
            LoadLocalFile(_imagePath);
        }
        else if (IsResourcesPath(_imagePath))
        {
            LoadResourcesPath(_imagePath);
        }
    }

    private bool IsLocalFilePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;

        var hasFileExtension = path.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                               path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                               path.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase);

        var isAbsolutePath = Path.IsPathRooted(path);
        var isPersistentPath = path.StartsWith(Application.persistentDataPath, StringComparison.OrdinalIgnoreCase);

        return hasFileExtension && (isAbsolutePath || isPersistentPath);
    }

    private bool IsResourcesPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;

        return char.IsLetter(path[0]) &&
               !path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
               !path.StartsWith("https://", StringComparison.OrdinalIgnoreCase) &&
               !path.StartsWith("file://", StringComparison.OrdinalIgnoreCase) &&
               !Path.IsPathRooted(path) &&
               !path.StartsWith(Application.persistentDataPath, StringComparison.OrdinalIgnoreCase);
    }

    private async UniTask LoadLocalFile(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                TriggerError();
                return;
            }

            byte[] imageBytes = await File.ReadAllBytesAsync(path);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageBytes))
            {
                SuccessTrugger(texture);
            }
            else TriggerError();
            Loading(false);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error loading image: {ex.Message}", "AsyncImageView");
            TriggerError();
        }
    }

    private void LoadResourcesPath(string path)
    {
        Logger.TryCatch(() =>
        {
            string resourcePath = Path.ChangeExtension(path, null);
            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            if (texture != null)
            {
                SuccessTrugger(texture);
            }
            else
            {
                TriggerError();
            }
            Loading(false);
        });
    }

    private void UpdateAspectRatio(Texture2D texture)
    {
        if (texture == null || aspectRatioFitter == null) return;

        float aspectRatio = (float)texture.width / texture.height;
        aspectRatioFitter.aspectRatio = aspectRatio;

        aspectRatioFitter.aspectMode = widthFill
            ? AspectRatioFitter.AspectMode.WidthControlsHeight
            : (texture.width > texture.height
                ? AspectRatioFitter.AspectMode.WidthControlsHeight
                : AspectRatioFitter.AspectMode.HeightControlsWidth);
    }

    private void SuccessTrugger(Texture2D texture) 
    {
        _image.texture = texture;
        UpdateAspectRatio(texture);
        TriggerAction(_imagePath);
        _image.color = Color.white;
        Placeholder(false);
    }

    private void TriggerError() 
    {
        _image.texture = null;
        TriggerAction<string>(null);
        _image.color = Color.clear;
        Placeholder(true);
    }

    private void Placeholder(bool val = true) 
    {
        if(_placeholder != null) _placeholder.SetActive(val);
    }

    private void Loading(bool val = true) 
    {
        if (_loading != null) _loading.SetActive(val);
    }
}