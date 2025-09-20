using UnityEngine;
using Cysharp.Threading.Tasks;

public class NotificationController : MonoBehaviour
{
    private NotificationManager _notificationManager;

    private void Start()
    {
        _notificationManager = DataCore.Instance.NotificationManager;
        RebuildNotificationQueue(); 
    }

    public async UniTask RequestNotificationPermission()
    {
        bool granted = await _notificationManager.RequestNotificationPermissionAsync();
        if (granted)
        {
            Logger.Log("Notification permission granted.");
            RebuildNotificationQueue();
        }
        else
        {
            Logger.LogWarning("Notification permission denied.");
        }
    }

    public void CreateNotificationForReservation(ReservationModel reservation)
    {
        _notificationManager.AddNotificationForReservation(reservation);
        Logger.Log($"Notification scheduled for reservation {reservation.Id}");
    }

    public void CompleteNotification(int id)
    {
        _notificationManager.MarkNotificationAsCompleted(id);
        Logger.Log($"Notification {id} marked as completed.");
    }

    public void DeleteNotification(int id)
    {
        _notificationManager.RemoveNotification(id);
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
        _notificationManager.RebuildNotificationQueue();
        Logger.Log("Notification queue rebuilt from controller.");
    }

    public void ClearAllNotifications()
    {
        _notificationManager.ClearNotifications();
        Logger.Log("All notifications cleared.");
    }
}