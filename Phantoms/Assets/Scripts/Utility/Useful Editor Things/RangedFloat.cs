using System;

[Serializable]
public struct RangedFloat
{
    public float minValue;
    public float maxValue;

    public RangedFloat(float value)
    {
        minValue = value;
        maxValue = value;
    }

    public RangedFloat(float minVal, float maxVal)
    {
        minValue = minVal;
        maxValue = maxVal;
    }
}
