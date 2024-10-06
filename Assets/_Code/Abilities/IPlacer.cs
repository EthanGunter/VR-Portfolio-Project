using UnityEngine;

public interface IPlacer
{
    public struct Transform { public Vector3 position; public Quaternion rotation; }
    Transform GetValidPlacement();
    void Place(IPlacer.Transform location);
}