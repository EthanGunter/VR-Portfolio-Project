using UnityEngine;

public class PlacementPoint : MonoBehaviour
{
    [SerializeField] Transform PlacePosition;

    public void Place(GameObject go, bool keepYRotation = false)
    {
        if (keepYRotation)
        {
            float yRot = go.transform.eulerAngles.y;
            Vector3 eulerRot = PlacePosition.eulerAngles;
            Quaternion rotation = Quaternion.Euler(eulerRot.x, yRot, eulerRot.z);
            go.transform.SetPositionAndRotation(PlacePosition.transform.position, rotation);
        }
        else
        {
            go.transform.SetPositionAndRotation(PlacePosition.transform.position, PlacePosition.transform.rotation);
        }
    }
}