using UnityEngine;

public interface IPlaceable
{
    bool IsPlacementValid();
    bool Place();
}