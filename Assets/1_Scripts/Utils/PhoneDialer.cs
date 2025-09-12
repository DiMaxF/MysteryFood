using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhoneDialer
{
    public static void OpenDialer(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            Debug.LogError("Номер телефона пустой!");
            return;
        }

        string formattedNumber = phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        if (!formattedNumber.StartsWith("+") && formattedNumber.Length > 0)
        {
            formattedNumber = "+" + formattedNumber; 
        }

        string uri = "tel:" + formattedNumber;
        Application.OpenURL(uri);
    }
}
