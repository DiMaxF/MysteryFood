using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Unity.Notifications;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;

public class NotificationController : MonoBehaviour
{
    private NotificationManager _notificationManager;
    private PersonalManager _personalManager;
    private const string CHANNEL_ID = "channel0";

    private void Start()
    {
        _notificationManager = DataCore.Instance.NotificationManager;
        _personalManager = DataCore.Instance.PersonalManager;
        InitializeNotifications();
        RebuildNotificationQueue();
    }

    private void InitializeNotifications()
    {
#if UNITY_ANDROID
        var channel = new AndroidNotificationChannel()
        {
            Id = CHANNEL_ID,
            Name = "Default Channel",
            Importance = Importance.High, 
            Description = "Default notification channel for MysteryFood", 
            CanShowBadge = true,
            EnableVibration = true 
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        AndroidNotificationCenter.Initialize();
#elif UNITY_IOS
        iOSNotificationCenter.Initialize();
#endif
    }

    public async UniTask<bool> RequestNotificationPermission()
    {
#if UNITY_IOS
        var currentStatus = iOSNotificationCenter.GetAuthorizationStatus();
        if (currentStatus != AuthorizationStatus.Authorized)
        {
            var result = await iOSNotificationCenter.RequestAuthorizationAsync(
                AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound);
            _notificationManager.Permission = result;
            if (result)
            {
                Logger.Log("Notification permission granted.");
                RebuildNotificationQueue();
            }
            else
            {
                Logger.LogWarning("Notification permission denied.");
            }
            return result;
        }
        _notificationManager.Permission = true;
        return true;
#elif UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            await UniTask.WaitUntil(
                () => Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"));
            _notificationManager.Permission = Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"); // Исправлено: AppData вместо Permission
            if (_notificationManager.Permission)
            {
                Logger.Log("Notification permission granted.");
                RebuildNotificationQueue();
            }
            else
            {
                Logger.LogWarning("Notification permission denied.");
            }
            return _notificationManager.Permission;
        }
        _notificationManager.Permission = true;
        return true;
#else
        _notificationManager.Permission = false;
        return false;
#endif
    }

    public void TestNotification()
    {
        var testNotification = new NotificationModel(UnityEngine.Random.Range(1000, 9999), "Test push", "The test push notification", DateTime.Now.AddSeconds(2));
        ScheduleNotification(testNotification);
        Logger.Log($"Test notification created with ID {testNotification.Id}");
    }

    public void CreateNotificationForReservation(ReservationModel reservation)
    {
        _notificationManager.AddNotificationForReservation(reservation);
        var notification = _notificationManager.GetActiveNotifications().FirstOrDefault(n => n.Id == reservation.Id);
        if (notification != null)
        {
            ScheduleNotification(notification);
            Logger.Log($"Notification scheduled for reservation {reservation.Id}");
        }
    }

    public void CompleteNotification(int id)
    {
        _notificationManager.MarkNotificationAsCompleted(id);
        CancelNotification(id);
        Logger.Log($"Notification {id} marked as completed.");
    }

    public void DeleteNotification(int id)
    {
        _notificationManager.RemoveNotification(id);
        CancelNotification(id);
        Logger.Log($"Notification {id} deleted.");
    }

    public void DisplayActiveNotifications()
    {
        var notifications = _notificationManager.GetActiveNotifications();
        foreach (var notification in notifications)
        {
            Logger.Log($"Active Notification: {notification.Title} - {notification.Message} at {notification.FireTime}");
        }
    }

    public void RebuildNotificationQueue()
    {
        ClearNotifications();
        _notificationManager.RebuildNotificationQueue();
        foreach (var notification in _notificationManager.GetActiveNotifications())
        {
            ScheduleNotification(notification);
        }
        Logger.Log("Notification queue rebuilt from controller.");
    }

    public void ClearNotifications()
    {
        foreach (var notification in _notificationManager.GetActiveNotifications())
        {
            CancelNotification(notification.Id);
        }
        Logger.Log("All notifications cleared.");
    }

    private void ScheduleNotification(NotificationModel notification)
    {
        if (notification.FireTime <= DateTime.Now)
        {
            Logger.LogWarning($"Cannot schedule notification {notification.Id}: Fire time is in the past.");
            return;
        }
        if (_personalManager.Notification < 0) return;
#if UNITY_ANDROID
        var androidNotification = new AndroidNotification
        {
            Title = notification.Title,
            Text = notification.Message,
            FireTime = notification.FireTime,
            SmallIcon = "icon_0",
            LargeIcon = "icon_1"
        };
        AndroidNotificationCenter.SendNotificationWithExplicitID(androidNotification, CHANNEL_ID, notification.Id);
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
}