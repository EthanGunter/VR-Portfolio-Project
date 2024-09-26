using SolarStorm.Entities;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] HealthComponent healthSource;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        if (healthSource == null && !TryGetComponent(out healthSource))
        {
            this.enabled = false;
            throw new MissingReferenceException($"{name} HealthBar component does not have the required HealthComponent reference. Disabling");
        }
    }

    private void Update()
    {
        slider.value = healthSource.Health / healthSource.MaxHealth;
        transform.LookAt(Camera.main.transform);
    }
}
