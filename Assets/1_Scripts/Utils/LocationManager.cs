using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;


public class LocationManager : MonoBehaviour
{
    [SerializeField] private float desiredAccuracyInMeters = 10f;
    [SerializeField] private float updateDistanceInMeters = 5f;  
    [SerializeField] private float timeoutSeconds = 10f;         

    private bool isRequesting = false;

    public async UniTask<GeoPoint> GetLocationAsync(CancellationToken cancellationToken = default)
    {
        if (isRequesting)
        {
            Debug.LogWarning("Геолокация уже запрашивается!");
            return null;
        }

        isRequesting = true;
        try
        {
            if (!await CheckAndRequestPermissionAsync(cancellationToken))
            {
                Debug.LogError("Разрешение на геолокацию не получено.");
                return null;
            }

            if (!Input.location.isEnabledByUser)
            {
                Debug.LogError("GPS выключен на устройстве. Включи в настройках.");
                return null;
            }

            Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);

            await WaitForLocationAsync(cancellationToken);

            if (Input.location.status != LocationServiceStatus.Running)
            {
                Debug.LogError($"GPS не запустился: статус {Input.location.status}");
                return null;
            }

            LocationInfo location = Input.location.lastData;
            string address = null;

            if (!cancellationToken.IsCancellationRequested)
            {
                address = await ReverseGeocodeAsync(location.latitude, location.longitude, cancellationToken);
            }

            GeoPoint geoPoint = new GeoPoint(
                location.latitude,
                location.longitude,
                 address ?? "Текущая позиция"
            );

            return geoPoint;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка получения геолокации: {ex.Message}");
            return null;
        }
        finally
        {
            Input.location.Stop();
            isRequesting = false;
        }
    }

    private async UniTask<bool> CheckAndRequestPermissionAsync(CancellationToken cancellationToken)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            await UniTask.WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.FineLocation) || cancellationToken.IsCancellationRequested, cancellationToken: cancellationToken);
            return Permission.HasUserAuthorizedPermission(Permission.FineLocation) && !cancellationToken.IsCancellationRequested;
        }
#endif
        return true;
    }

    private async UniTask WaitForLocationAsync(CancellationToken cancellationToken)
    {
        float elapsed = 0f;
        while (Input.location.status == LocationServiceStatus.Initializing && elapsed < timeoutSeconds)
        {
            await UniTask.Yield(cancellationToken);
            elapsed += Time.deltaTime;
        }

        if (elapsed >= timeoutSeconds)
        {
            Debug.LogError("Таймаут ожидания GPS.");
        }
    }

    private async UniTask<string> ReverseGeocodeAsync(float latitude, float longitude, CancellationToken cancellationToken)
    {
        string url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude}&lon={longitude}&zoom=18&addressdetails=1";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("User-Agent", "UnityApp/1.0"); 

            var operation = request.SendWebRequest();
            await UniTask.WaitUntil(() => operation.isDone, cancellationToken: cancellationToken);

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                int startIndex = json.IndexOf("\"display_name\":\"") + 15;
                int endIndex = json.IndexOf("\"", startIndex);
                if (startIndex > 15 && endIndex > startIndex)
                {
                    return json.Substring(startIndex, endIndex - startIndex);
                }
                Debug.LogWarning("Не удалось извлечь адрес из JSON: " + json);
                return null;
            }
            else
            {
                Debug.LogError($"Ошибка Nominatim: {request.error}");
                return null;
            }
        }
    }
    /*
    private async UniTaskVoid ExampleUsage()
    {
        GeoPoint point = await GetLocationAsync();
        if (point != null)
        {
            OnlineMaps.instance.position = point.ToVector2();
            OnlineMaps.instance.zoom = 15;

            OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(point.ToVector2());
            marker.label = point.Address;

            Debug.Log($"GeoPoint: {point.Address} ({point.Latitude}, {point.Longitude})");
        }
    }

    public void RequestLocation()
    {
        ExampleUsage().Forget();
    }*/

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Input.location.Stop();
            isRequesting = false;
        }
    }

    void OnDestroy()
    {
        Input.location.Stop();
    }
}
