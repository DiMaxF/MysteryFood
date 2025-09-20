using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Android;

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


            GeoPoint geoPoint = new GeoPoint(
                location.latitude,
                location.longitude,
                 address ?? "UserPosition"
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

    public async UniTask<bool> CheckAndRequestPermissionAsync(CancellationToken cancellationToken = default)
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
