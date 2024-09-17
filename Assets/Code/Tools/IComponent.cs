using UnityEngine;

public interface IComponent
{
    public Transform transform { get; }
    public GameObject gameObject { get; }
    public string tag { get; }
}