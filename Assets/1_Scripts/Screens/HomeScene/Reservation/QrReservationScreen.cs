using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class QrReservationScreen : AppScreen
{
    [SerializeField] private ButtonView _back;
    [SerializeField] private ButtonView _fullInfo;
    [SerializeField] private ButtonView _markedAsPickedUp;
    [SerializeField] private AsyncImageView _qrImage;
    [SerializeField] private ButtonView _hintNotification;
    [SerializeField] private ReservationShortView _reservationInfo;
    [Header("Overlay")]
    [SerializeField] private ConfirmPanel _confirm;
    private ReservationModel _model;
    public void SetModel(ReservationModel model) => _model = model;

    protected override void OnStart()
    {
        base.OnStart();
        UIContainer.RegisterView(_confirm); 
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        UIContainer.InitView(_reservationInfo, _model);
        UIContainer.InitView(_qrImage, _model.QrPath);
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView(_back, (object _) => OnButtonBack());
        UIContainer.SubscribeToView(_confirm, (object _) => Container.Back().Forget());
        UIContainer.SubscribeToView(_fullInfo, (object _) => OnButtonFullInfo());
        UIContainer.SubscribeToView(_markedAsPickedUp, (object _) => OnButtonMarked());

    }

    private void OnButtonBack()
    {
        UIContainer.InitView(_confirm, "Outside pickup window. Confirm?");
        _confirm.Show();
    }

    private void OnButtonFullInfo() 
    {
        var screen = Container.GetScreen<OrderDetailsScreen>();
        screen.SetModel(_model);
        Container.Show(screen);
    }

    private void OnButtonMarked()
    {
        _model.Status = StatusReservation.PickedUp;
        Data.ReservationManager.Update(_model);
        Data.SaveData();
    }
}
