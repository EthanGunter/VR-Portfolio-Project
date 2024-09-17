using UnityEngine;

public interface IPlaceable
{
    bool IsPlacementValid();
    void Place();
}