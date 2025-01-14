using SolarStorm.UnityToolkit;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

public class Player : MonoBehaviour
{
    #region Variables

    [SerializeField] ScriptableEvent<float> playerHeightChanged;

    private InputDeviceCharacteristics leftController = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left;
    private InputDeviceCharacteristics rightController = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right;
    private InputDeviceCharacteristics HMD = InputDeviceCharacteristics.HeadMounted | InputDeviceCharacteristics.TrackedDevice;

    #endregion

    public static XROrigin XROrigin { get; private set; }
    public static Transform Head { get; private set; }

    [SerializeField] float minimumStandingHeight = 1.5f;
    [SerializeField] float desiredStandingHeight = 1.65f;


    #region Unity Messages

    private void Awake()
    {
        XROrigin = Object.FindFirstObjectByType<XROrigin>();
        Head = Object.FindFirstObjectByType<Camera>().transform;
    }

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    #endregion

    private IEnumerator Initialize()
    {
        while (!AreAllDevicesConnected())
        {
            Debug.Log("Waiting for devices to connect...", this);
            yield return null;
        }
        Debug.Log("All connected! Initializing...", this);

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

    private bool AreAllDevicesConnected()
    {
        List<InputDevice> devices = new();
        InputDevices.GetDevicesWithCharacteristics(leftController, devices);
        if (devices.Count < 1) return false;
        InputDevices.GetDevicesWithCharacteristics(rightController, devices);
        if (devices.Count < 1) return false;
        InputDevices.GetDevicesWithCharacteristics(HMD, devices);
        if (devices.Count < 1) return false;

        return true;
    }
}