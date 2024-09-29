using SolarStorm.UnityToolkit;
using UnityEngine;

public class BelongsToSet : MonoBehaviour
{
    [SerializeField] GameObjectSet[] _runtimeSets;
    [Tooltip("If true, this object will be accessible from runtime sets, even if it is not active")]
    [SerializeField] readonly bool _inSetWhenDisabled;

    private void Awake()
    {
        if (_inSetWhenDisabled) AddToSets();
    }
    private void OnDestroy()
    {
        if (_inSetWhenDisabled) RemoveFromSets();
    }
    private void OnEnable()
    {
        if (!_inSetWhenDisabled) AddToSets();
    }
    private void OnDisable()
    {
        if (!_inSetWhenDisabled) RemoveFromSets();
    }

    private void AddToSets()
    {
        foreach (var set in _runtimeSets)
        {
            set.Add(gameObject);
        }
    }
    private void RemoveFromSets()
    {
        foreach (var set in _runtimeSets)
        {
            set.Remove(gameObject);
        }
    }
}