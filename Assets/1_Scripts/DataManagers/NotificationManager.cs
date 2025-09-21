using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

public class NotificationManager : IDataManager
{
    private readonly AppData _appData;

    public bool Permission 
        {
           get => _appData.PermissionNotification;  
        set => _appData.PermissionNotification = value; 
    }

    public NotificationManager(AppData appData)
    {
        _appData = appData ?? throw new ArgumentNullException(nameof(appData));
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
        DataCore.Instance.SaveData();
    }

    public void RebuildNotificationQueue()
    {
        _appData.Notifications.Clear();
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
        }
    }

    public void RemoveNotification(int id)
    {
        var notification = _appData.Notifications.FirstOrDefault(n => n.Id == id);
        if (notification != null)
        {
            _appData.Notifications.Remove(notification);
        }
    }

    public List<NotificationModel> GetActiveNotifications()
    {
        return _appData.Notifications
            .Where(n => !n.IsCompleted && n.FireTime >= DateTime.Now)
            .OrderBy(n => n.FireTime)
            .ToList();
    }

    public void Clear()
    {
        
    }
}