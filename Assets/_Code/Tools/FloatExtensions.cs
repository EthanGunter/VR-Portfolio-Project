using UnityEngine;

public static class FloatExtensions
{
    public static float Remap(this float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
    }
}