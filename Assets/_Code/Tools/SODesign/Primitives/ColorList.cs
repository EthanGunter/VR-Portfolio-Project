using SolarStorm.UnityToolkit;
using UnityEngine;

[CreateAssetMenu(menuName ="Shared Data/Primitives/Color List")]
public class ColorList : ScriptableObject
{
    [SerializeField] [ColorUsage(true, true)] public Color[] colors;
}