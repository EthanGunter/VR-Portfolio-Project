using SolarStorm.Entities;
using UnityEngine;

public class TimedHealth : HealthComponent
{
    [Tooltip("Time in seconds")]
    [SerializeField] protected float aliveTime = 10;

    private float drainPerSecond;

    private void OnEnable()
    {
        drainPerSecond = MaxHealth / aliveTime;
    }

    void Update()
    {
        Health -= Time.deltaTime * drainPerSecond;
    }
}
