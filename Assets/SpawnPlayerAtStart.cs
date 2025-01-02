using Unity.XR.CoreUtils;
using UnityEngine;

public class SpawnPlayerAtStart : MonoBehaviour
{

    [SerializeField] XROrigin playerOrigin;
    [SerializeField] SetPedestalHeight pedestalSetter;
    [SerializeField] float minimumStandingHeight = 1.5f;
    [SerializeField] float desiredStandingHeight = 1.65f;
    private void Start()
    {
        // Detect player height, then assume whether they are sitting or not        
        // Get the height of the camera, and calculate how high it is off the ground
        float detectedHeight = playerOrigin.GetComponentInChildren<Camera>().transform.position.y - playerOrigin.transform.position.y;

        Debug.Log($"Detected height: {detectedHeight}", this);
        if (detectedHeight < minimumStandingHeight)
        {
            Debug.Log("We're probably sitting", this);
            // We're probably sitting
            playerOrigin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Device;
            playerOrigin.CameraYOffset = desiredStandingHeight;
        }
        else
        {
            Debug.Log("We're probably standing", this);
            // We're probably standing, so nothing to do here besides set the pedestal to the right height
        }

        // Move the player to the spawn position
        playerOrigin.transform.position = new Vector3(transform.position.x, playerOrigin.transform.position.y, transform.position.z);

        // Raise the pedestal so it's at an easy reading height
        pedestalSetter.SetHeight(playerOrigin.CameraYOffset);
    }
}
