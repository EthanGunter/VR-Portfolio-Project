using UnityEngine;

public static class LayerMaskExtensions
{
    public static bool Contains(this LayerMask mask, int layer)
    {
        return (0 != (mask & (1 << layer)));
    }

    public static bool Contains(this LayerMask mask, GameObject gob)
    {
        if (gob == null)
            return false;
        return (0 != (mask & (1 << gob.layer)));
    }
    public static void Add(ref this LayerMask mask, int layer)
    {
        mask |= (1 << layer);
    }

    public static void Remove(ref this LayerMask mask, int layer)
    {
        mask &= ~(1 << layer);
    }
}