using UnityEngine;

namespace SolarStorm.UnityToolkit
{
    [CreateAssetMenu(menuName = "Shared Data/Runtime Collections/Game Objects")]
    public class GameObjectRuntimeSet : RuntimeSet<GameObject>
    {
        // TODO? - automatic physics layer assignment
        //public bool useLayerMask = false;
        //[Tooltip("Automatically puts gameobjects on this layer when added to the list. " +
        //    "Objects added to multiple gameobject sets that use layer masks will use whichever it is added to last...")]
        //public LayerMask Layer;
    }
}
