using System;
[Serializable]
public class ReservationModel
{
    public int Id;
    public int VenueId;
    public int Quantity;
    public CurrencyModel OriginalPrice;
    public CurrencyModel DiscountedPrice;
    public string Notes;
    public string QrPath;
    public bool Notification;
    public StatusReservation Status;
    public string CreatedAt;
}
