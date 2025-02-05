using Sirenix.OdinInspector.Editor.TypeSearch;
using SolarStorm.UnityToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR.Input;

public class Player : Singleton<Player>
{
    #region Variables

    public static XROrigin XROrigin { get; private set; }
    public static Transform Head { get; private set; }
    public static bool AllDevicesConnected => HMDConnected && LeftControllerConnected && RightControllerConnected;
    public static bool HMDConnected { get; private set; }
    public static bool LeftControllerConnected { get; private set; }
    public static bool RightControllerConnected { get; private set; }

    [SerializeField] float minimumStandingHeight = 1.5f;
    [SerializeField] float desiredStandingHeight = 1.65f;
    [SerializeField] ScriptableEvent<float> playerHeightChanged;

    #endregion



    #region Unity Messages

    protected override void Awake()
    {
        base.Awake();
        XROrigin = Object.FindFirstObjectByType<XROrigin>();
        Head = Object.FindFirstObjectByType<Camera>().transform;
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    #endregion

    public static void RequestHeightUpdate()
    {
        instance.playerHeightChanged.Invoke(Player.XROrigin.CameraYOffset);
    }

    private IEnumerator Initialize()
    {
        while (!HMDConnected)
        {
            Debug.Log("Waiting for devices to connect...", this);
            yield return null;
        }

        Debug.Log("HMD connected! Initializing...", this);

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
        RequestHeightUpdate();
    }

    private void OnDeviceChange(UnityEngine.InputSystem.InputDevice device, InputDeviceChange state)
    {
        //Debug.Log($"({device.GetType().Name}) is now {state} | {string.Join(", ", device.usages.Select(x => x.ToString() + ", "))}", this);

        if (state == InputDeviceChange.Enabled || state == InputDeviceChange.Reconnected)
        {
            if (device is TrackedDevice)
            {
                //Debug.Log("HEADSET CONNECTED", this);
                HMDConnected = true;
            }
            else if (device is XRController)
            {
                if (device.usages.Any(x => x.Equals("RightHand")))
                {
                    //Debug.Log("RIGHT CTRL CONNECTED", this);
                    RightControllerConnected = true;
                }
                if (device.usages.Any(x => x.Equals("LeftHand")))
                {
                    //Debug.Log("LEFT CTRL CONNECTED", this);
                    LeftControllerConnected = true;
                }
            }
        }
        else if (state == InputDeviceChange.Disabled || state == InputDeviceChange.Disconnected)
        {
            if (device is XRHMD)
            {
                //Debug.Log("HEADSET LOST", this);
                HMDConnected = false;
            }
            else if (device is XRController)
            {
                if (device.usages.Any(x => x.Equals("RightHand")))
                {
                    //Debug.Log("RIGHT CTRL LOST", this);
                    RightControllerConnected = false;
                }
                if (device.usages.Any(x => x.Equals("LeftHand")))
                {
                    //Debug.Log("LEFT CTRL LOST", this);
                    LeftControllerConnected = false;
                }
            }
        }
    }
}