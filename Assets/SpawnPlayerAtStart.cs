using Unity.XR.CoreUtils;
using UnityEngine;

public class SpawnPlayerAtStart : MonoBehaviour
{
    private void Start()
    {
        // Move the player to the spawn position
        Player.XROrigin.transform.position = new Vector3(transform.position.x, Player.XROrigin.transform.position.y, transform.position.z);
    }
}
