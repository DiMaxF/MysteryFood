using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#endif
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    private const string ChannelId = "winamax_notification_channel";
    private const string SmallIconId = "notify_small";
    private const string LargeIconId = "notify_large";
    private const int BaseNotificationId = 1000;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

#if UNITY_ANDROID
        CreateNotificationChannel();
#endif
    }

#if UNITY_ANDROID
    private void CreateNotificationChannel()
    {
        var channel = new AndroidNotificationChannel
        {
            Id = ChannelId,
            Name = "Default Notifications",
            Importance = Importance.Default,
            Description = "General notifications",
            EnableVibration = true,
            CanShowBadge = true
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }
#endif

    public async UniTask<bool> CheckNotificationPermissionAsync()
    {
#if UNITY_ANDROID
        return Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS");
#elif UNITY_IOS
        var settings = await iOSNotificationCenter.GetNotificationSettingsAsync();
        return settings.AuthorizationStatus == AuthorizationStatus.Authorized;
#else
        return false;
#endif
    }

    public async UniTask<bool> RequestNotificationPermissionAsync()
    {
        bool hasPermission = await CheckNotificationPermissionAsync();
        if (hasPermission)
        {
            Debug.Log("Ðàçðåøåíèå íà óâåäîìëåíèÿ óæå ïîëó÷åíî");
            return true;
        }

#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            hasPermission = Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS");
        }
#endif

#if UNITY_IOS
        var request = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound, true);
        await UniTask.WaitUntil(() => request.IsFinished);
        hasPermission = request.Granted;
#endif
        return hasPermission;
    }

    public async UniTask<bool> ScheduleNotificationsAsync()
    {
        /*if (!DataCore.Instance.Notification)
        {
            Debug.Log("Óâåäîìëåíèÿ îòêëþ÷åíû â íàñòðîéêàõ");
            return false;
        }*/

        bool hasPermission = await CheckNotificationPermissionAsync();
        if (!hasPermission)
        {
            Debug.LogWarning("Óâåäîìëåíèÿ íå çàïëàíèðîâàíû: îòñóòñòâóåò ðàçðåøåíèå");
            return false;
        }

        ClearAllNotifications();

        int notificationId = BaseNotificationId;
        /*foreach (var eventModel in events)
        {
            if (!eventModel.notification || string.IsNullOrEmpty(eventModel.date))
                continue;

            DateTime eventDate = eventModel.Date;
            if (eventModel.everyYear)
            {
                eventDate = new DateTime(DateTime.Now.Year, eventDate.Month, eventDate.Day);
                if (eventDate < DateTime.Now)
                    eventDate = eventDate.AddYears(1);
            }

            if (eventModel.dayBefore && eventDate > DateTime.Now.AddDays(1))
            {
                await ScheduleSingleNotificationAsync(
                    notificationId++,
                    eventModel.name,
                    $"Notification: {eventModel.description}",
                    eventDate.AddDays(-1),
                    eventModel.soundOn
                );
            }

            if (eventDate >= DateTime.Now)
            {
                await ScheduleSingleNotificationAsync(
                    notificationId++,
                    eventModel.name,
                    eventModel.description,
                    eventDate,
                    eventModel.soundOn
                );
            }
        }*/

        Debug.Log($"Çàïëàíèðîâàíî {notificationId - BaseNotificationId} óâåäîìëåíèé");
        return true;
    }



    public void ClearAllNotifications()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllScheduledNotifications();
#endif
#if UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
        iOSNotificationCenter.ClearDeliveredNotifications();
#endif
        Debug.Log("Âñå çàïëàíèðîâàííûå óâåäîìëåíèÿ óäàëåíû");
    }

    public async UniTask<bool> ScheduleSingleNotificationAsync(int id, string title, string message, DateTime fireTime, bool soundOn)
    {
#if UNITY_ANDROID
        var notification = new AndroidNotification
        {
            Title = title,
            Text = message,
            SmallIcon = SmallIconId,
            LargeIcon = LargeIconId,
            FireTime = fireTime,
            ShouldAutoCancel = true
        };
        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, ChannelId, id);
        Debug.Log($"Óâåäîìëåíèå (Android, ID: {id}): {title} - {message}, çàïëàíèðîâàíî íà {fireTime:yyyy-MM-dd HH:mm:ss}, çâóê: {(soundOn ? "âêë (ñèñòåìíûé)" : "çàâèñèò îò ñèñòåìíûõ íàñòðîåê")}");
#endif

#if UNITY_IOS
    if (fireTime < DateTime.Now)
    {
        Debug.LogWarning($"Cannot schedule notification in the past: {fireTime:yyyy-MM-dd HH:mm:ss}");
        return false;
    }
    var iosNotification = new iOSNotification
    {
        Identifier = id.ToString(),
        Title = title,
        Body = message,
        ShowInForeground = true,
        Trigger = new iOSNotificationTimeIntervalTrigger
        {
            TimeInterval = fireTime - DateTime.Now,
            Repeats = false
        },
        SoundType = soundOn ? iOSNotificationSoundType.Default : iOSNotificationSoundType.None
    };
    iOSNotificationCenter.ScheduleNotification(iosNotification);
    Debug.Log($"Óâåäîìëåíèå (iOS, ID: {id}): {title} - {message}, çàïëàíèðîâàíî íà {fireTime:yyyy-MM-dd HH:mm:ss}, çâóê: {(soundOn ? "âêë" : "âûêë")}");
#endif
        return true;
    }
}