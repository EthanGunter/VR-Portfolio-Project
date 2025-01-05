using SolarStorm.UnityToolkit;
using Unity.XR.CoreUtils;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Variables

    [SerializeField] ScriptableEvent<float> playerHeightChanged;

    #endregion

    public static XROrigin XROrigin;

    [SerializeField] float minimumStandingHeight = 1.5f;
    [SerializeField] float desiredStandingHeight = 1.65f;


    #region Unity Messages

    private void Awake()
    {
        XROrigin = Object.FindFirstObjectByType<XROrigin>();
    }

    private void Start()
    {
        // Detect player height, then assume whether they are sitting or not        
        // Get the height of the camera, and calculate how high it is off the ground
        float detectedHeight = XROrigin.GetComponentInChildren<Camera>().transform.position.y - XROrigin.transform.position.y;

        //Debug.Log($"Detected height: {detectedHeight}", this);
        if (detectedHeight < minimumStandingHeight)
        {
            //Debug.Log("We're probably sitting", this);
            // We're probably sitting
            XROrigin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Device;
            XROrigin.CameraYOffset = desiredStandingHeight;
        }
        else
        {
            //Debug.Log("We're probably standing", this);
            // We're probably standing, so nothing to do here
        }

        // Raise the pedestal so it's at an easy reading height
        playerHeightChanged.Invoke(Player.XROrigin.CameraYOffset);
    }

    #endregion
}