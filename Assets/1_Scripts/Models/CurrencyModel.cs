using System;
using System.Collections.Generic;

[Serializable]
public class CurrencyModel
{
    public float Amount;
    public Currency Currency;

    private static readonly Dictionary<Currency, float> ExchangeRates = new Dictionary<Currency, float>
    {
        { Currency.USD, 1f },
        { Currency.EUR, 0.93f }, 
        { Currency.EGP, 48.5f }  
    };


    public CurrencyModel()
    {
        Amount = 0;
        Currency = Currency.USD;
    }


    public CurrencyModel(float amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public float ConvertTo(Currency targetCurrency)
    {
        if (Currency == targetCurrency) return Amount;

        float amountInUSD = Amount / ExchangeRates[Currency];
        return amountInUSD * ExchangeRates[targetCurrency];
    }

    public string ToString(Currency targetCurrency)
    {
        float convertedAmount = ConvertTo(targetCurrency);
        return $"{convertedAmount:F2} {targetCurrency}";
    }
    override public string ToString()
    {
        float convertedAmount = ConvertTo(Currency);
        return $"{convertedAmount:F2} {Currency}";
    }
}