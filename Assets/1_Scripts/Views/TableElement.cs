using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableElement : View
{
    [SerializeField] private Text _one;
    [SerializeField] private Text _two;
    [SerializeField] private Text _three;
    [SerializeField] private Text _four;
    [Serializable]
    public class Data 
    {
        public string one;
        public string two;
        public string three;
        public string four;
        public bool lable;

        public Data(string one, string two, string three, string four, bool lable = false) 
        {
            this.one = one;
            this.two = two;
            this.three = three;
            this.four = four;
            this.lable = lable;
        }
    }
    private Data _data;
    public override void UpdateUI()
    {
        base.UpdateUI();
        _one.text = _data.one;
        _two.text = _data.two;
        _three.text = _data.three;
        _four.text = _data.four;  
    }

    public override void Init<T>(T data)
    {
        if (data is Data d) _data = d;
        base.Init(data);
    }
}
