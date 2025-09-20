using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Notifications;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;

public class NotificationManager
{
    private readonly AppData _appData;

    public NotificationManager(AppData appData)
    {
        _appData = appData ?? throw new ArgumentNullException(nameof(appData));
        InitializeNotifications();
    }

    private void InitializeNotifications()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.Initialize();
#elif UNITY_IOS
        iOSNotificationCenter.Initialize();
#endif
    }

    public async UniTask<bool> RequestNotificationPermissionAsync(CancellationToken cancellationToken = default)
    {
#if UNITY_IOS
    var currentStatus = iOSNotificationCenter.GetAuthorizationStatus();
    if (currentStatus != AuthorizationStatus.Authorized)
    {
        var result = await iOSNotificationCenter.RequestAuthorizationAsync(
            AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound,
            cancellationToken: cancellationToken);

        _appData.PermissionNotification = result;
        return result && !cancellationToken.IsCancellationRequested;
    }
    _appData.PermissionNotification = true;
    return true;
#elif UNITY_ANDROID 
    if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
    {
        Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        await UniTask.WaitUntil(
            () => Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS") || cancellationToken.IsCancellationRequested,
            cancellationToken: cancellationToken);
        
        _appData.PermissionNotification = Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS");
        return _appData.PermissionNotification && !cancellationToken.IsCancellationRequested;
    }
    _appData.PermissionNotification = true;
    return true;
#else
        _appData.PermissionNotification = false;
        return false;
#endif
    }

    public void AddNotificationForReservation(ReservationModel reservation)
    {
        if (!_appData.PermissionNotification || _appData.Notification == -1 || !reservation.Notification)
        {
            Logger.LogWarning("Notifications are disabled or not allowed for this reservation.");
            return;
        }

        if (!DateTime.TryParse(reservation.StartTime, out DateTime startTime))
        {
            Logger.LogError($"Invalid StartTime format for reservation {reservation.Id}");
            return;
        }

        var id = reservation.Id;
        DateTime fireTime = startTime.AddMinutes(-_appData.Notification);
        string title = $"Reservation Reminder: {reservation.Id}";
        string message = $"Your reservation at Venue {reservation.VenueId} starts at {reservation.StartTime}.";

        var notification = new NotificationModel(id, title, message, fireTime);
        _appData.Notifications.RemoveAll(n => n.Id == id);
        _appData.Notifications.Add(notification);
        ScheduleNotification(notification);
        DataCore.Instance.SaveData();
    }

    public void RebuildNotificationQueue()
    {
        ClearNotifications();
        if (_appData.Notification == -1 || !_appData.PermissionNotification)
        {
            Logger.LogWarning("Notifications are disabled, queue not rebuilt.");
            return;
        }

        foreach (var reservation in _appData.Reservations)
        {
            if (reservation.Notification && reservation.Status == StatusReservation.Booked)
            {
                AddNotificationForReservation(reservation);
            }
        }
        Logger.Log("Notification queue rebuilt.");
    }

    public void MarkNotificationAsCompleted(int id)
    {
        var notification = _appData.Notifications.FirstOrDefault(n => n.Id == id);
        if (notification != null)
        {
            notification.IsCompleted = true;
            CancelNotification(id);
        }
    }

    public void RemoveNotification(int id)
    {
        var notification = _appData.Notifications.FirstOrDefault(n => n.Id == id);
        if (notification != null)
        {
            _appData.Notifications.Remove(notification);
            CancelNotification(id);
        }
    }

    public List<NotificationModel> GetActiveNotifications()
    {
        return _appData.Notifications
            .Where(n => !n.IsCompleted && n.FireTime >= DateTime.Now)
            .OrderBy(n => n.FireTime)
            .ToList();
    }

    private void ScheduleNotification(NotificationModel notification)
    {
        if (notification.FireTime <= DateTime.Now)
        {
            Logger.LogWarning($"Cannot schedule notification {notification.Id}: Fire time is in the past.");
            return;
        }

#if UNITY_ANDROID
        var androidNotification = new AndroidNotification
        {
            Title = notification.Title,
            Text = notification.Message,
            FireTime = notification.FireTime,
            SmallIcon = "icon_0",
            LargeIcon = "icon_1"
        };
        AndroidNotificationCenter.SendNotificationWithExplicitID(androidNotification, "channel0", notification.Id);
#elif UNITY_IOS
        var iosNotification = new iOSNotification
        {
            Identifier = notification.Id,
            Title = notification.Title,
            Body = notification.Message,
            Trigger = new iOSNotificationTimeIntervalTrigger
            {
                TimeInterval = notification.FireTime - DateTime.Now,
                Repeats = false
            }
        };
        iOSNotificationCenter.ScheduleNotification(iosNotification);
#endif
    }

    private void CancelNotification(int id)
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelScheduledNotification(id);
#elif UNITY_IOS
        iOSNotificationCenter.RemoveScheduledNotification(id);
#endif
    }

    public void ClearNotifications()
    {
        foreach (var notification in _appData.Notifications)
        {
            CancelNotification(notification.Id);
        }
        _appData.Notifications.Clear();
    }
}