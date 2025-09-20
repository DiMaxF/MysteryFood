using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class SliderData
{
    public float Min;
    public float Max;
    public float Current;

    public SliderData(float min, float max)
    {
        Min = min;
        Max = max;
    }
}
