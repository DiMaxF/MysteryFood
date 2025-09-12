using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VenueListView : View
{
    [SerializeField] private AsyncImageView image;
    [SerializeField] private Text address;
    [SerializeField] private Text name;
    [SerializeField] private Text price;
    [SerializeField] private Text distance;
    [SerializeField] private Text pickupToday;

    private VenueModel _model;
    
    public override void Init<T>(T data)
    {
        if (data is VenueModel venue) 
        {
            _model = venue;
        }
        //DataCore.Instance.Personal.Gep
        base.Init(data);
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        if (_model != null) 
        {
            UIContainer.InitView(image, _model.ImagePath);
            price.text = _model.Price.ToString();
        }
    }
}
