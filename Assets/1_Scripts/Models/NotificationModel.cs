using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NotificationModel
{
    public int Id;
    public string Title;
    public string Message;
    public DateTime FireTime;
    public bool IsCompleted;

    public NotificationModel(int id, string title, string message, DateTime fireTime, bool isCompleted = false)
    {
        Id = id;
        Title = title;
        Message = message;
        FireTime = fireTime;
        IsCompleted = isCompleted;
    }
}