using SolarStorm.UnityToolkit;
using UnityEngine;

[CreateAssetMenu(menuName = "Turret Level Data")]
public class TurretLevelData : ScriptableObject
{
    [ColorUsage(false, true)] public Color emissionColor;
    public FloatRef damagePerShot = 1;
    public FloatRef projectileSpeed = 1;
    public FloatRef shotsPerMinute = 30;
    public FloatRef turnSpeed = 1;
    public FloatRef maxTargetingDistance = 20;

    // TODO Should levels hold projectile references instead of data?
}