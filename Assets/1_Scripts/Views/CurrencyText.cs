using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyText : MonoBehaviour
{
    [SerializeField] private Text _text;
    DataCore _data => DataCore.Instance;

    private void OnEnable()
    {
        _text.text = _data.PersonalManager.Currency.ToString(); 
    }
}
