using SolarStorm.UnityToolkit;
using UnityEngine;

[System.Serializable]
public class InterfaceReference
{
    [SerializeField] protected MonoBehaviour _interfaceObject;
}

[System.Serializable]
public class InterfaceRef<T> : InterfaceReference where T : class
{
    public T Value => _interfaceObject as T;
}