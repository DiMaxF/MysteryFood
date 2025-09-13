using System;
using Unity.VisualScripting;
using static OnlineMapsHereRoutingAPIResult;
[Serializable]
public class ReservationModel
{
    public int Id;
    public int VenueId;
    public int Quantity;
    public CurrencyModel OriginalPrice;
    public CurrencyModel DiscountedPrice;
    public string StartTime;
    public string EndTime;
    public string Notes;
    public string QrPath;
    public bool Notification;
    public StatusReservation Status;
    public string CreatedAt;

    public ReservationModel()
    {
        Quantity = 0;
        OriginalPrice = new CurrencyModel();
        DiscountedPrice = new CurrencyModel();
        StartTime = string.Empty;
        EndTime = string.Empty;
        Notes = string.Empty;
        QrPath = string.Empty;
        Notification = true;
        Status = StatusReservation.Booked;
    }
}
