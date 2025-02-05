using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SolarStorm.UnityToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class LocomotionController : Singleton<LocomotionController>
{
    #region Variables

    [SerializeField] Transform LocomotionRoot;

    [SerializeField] ControllerInputActionManager _leftController;
    [SerializeField] ControllerInputActionManager _rightController;

    public static event Action OnControlSchemeChanged;

    private static ControllerInputActionManager leftController;
    private static ControllerInputActionManager rightController;

    private static DynamicMoveProvider slideProvider;

    private static TeleportationProvider teleportProvider;
    private static XRRayInteractor leftTeleRay;
    private static XRRayInteractor rightTeleRay;

    private static ContinuousTurnProvider smoothTurnProvider;

    private static SnapTurnProvider snapTurnProvider;

    private static TwoHandedGrabMoveProvider grabProvider;

    // TODO These should be combined since Linq exists
    private static LocomotionControlData[] _leftControls = { new SlideMoveData() { hand = Hand.left } };
    private static LocomotionControlData[] _rightControls = { new SmoothTurnData() { hand = Hand.right } };

    #endregion


    #region Unity Messages

    protected override void Awake()
    {
        base.Awake();

        if (!LocomotionRoot) throw new MissingComponentException("The Locomotion root must be defined, or no controls will be found!");

        if (!_leftController) throw new MissingComponentException("Left controller not connected");
        else
        {
            leftController = _leftController;
            leftTeleRay = leftController.GetComponentsInChildren<XRRayInteractor>(true).FirstOrDefault(x => x.interactionLayers.Contains(LayerMask.NameToLayer("teleport")));
            if (leftTeleRay == null)
            {
                Debug.LogError(new MissingComponentException("There is no XRRayInteractor component with 'teleport' interaction in the left controller's hierarchy"), this);
            }
        }

        if (!_rightController) throw new MissingComponentException("Right controller not connected");
        else
        {
            rightController = _rightController;
            var children = rightController.GetComponentsInChildren<XRRayInteractor>(true);
            rightTeleRay = children.FirstOrDefault(x => x.interactionLayers.Contains(LayerMask.NameToLayer("teleport")));
            if (rightTeleRay == null)
            {
                Debug.LogError(new MissingComponentException("There is no XRRayInteractor component with 'teleport' interaction in the left controller's hierarchy"), this);
            }
        }


        slideProvider = LocomotionRoot.GetComponentInChildren<DynamicMoveProvider>();
        teleportProvider = LocomotionRoot.GetComponentInChildren<TeleportationProvider>();

        smoothTurnProvider = LocomotionRoot.GetComponentInChildren<ContinuousTurnProvider>();
        snapTurnProvider = LocomotionRoot.GetComponentInChildren<SnapTurnProvider>();

        grabProvider = LocomotionRoot.GetComponentInChildren<TwoHandedGrabMoveProvider>();
    }

    private void Start()
    {
        UpdateControlScheme();
    }

    #endregion


    public static LocomotionControlData[] GetControlScheme()
    {
        List<LocomotionControlData> data = new();
        data.AddRange(_leftControls);
        data.AddRange(_rightControls);
        return data.ToArray();
    }
    public static LocomotionControlData GetHandControl<T>(Hand hand) where T : LocomotionControlData
    {
        if (hand == Hand.left)
        {
            return _leftControls.First(x => x.GetType() == typeof(T));
        }
        else if (hand == Hand.right)
        {
            return _rightControls.First(x => x.GetType().Equals(typeof(T)));
        }
        return null;
    }

    public static void SetControl(LocomotionControlData mode)
    {
        // Get relevant active controls
        LocomotionControlData[] controls = null;
        if (mode is HandedControlData hand)
        {
            if (hand.hand == Hand.left) controls = _leftControls;
            else if (hand.hand == Hand.right) controls = _rightControls;
            else throw new InvalidOperationException("???");

            if (mode is SlideMoveData slide)
            {
                // Allow no other controls on this controller
                controls = new LocomotionControlData[] { slide };
            }
            else if (mode is TeleportControlData tele)
            {
                // Allow any existing rotation on this controller
                controls = controls.Where(x => x is TurnData).Append(mode).ToArray();
            }
            else if (mode is SnapTurnData snap)
            {
                // Allow only teleport on this controller
                controls = controls.Where(x => x is TeleportControlData).Append(mode).ToArray();
            }
            else if (mode is SmoothTurnData smooth)
            {
                // Allow only teleport on this controller
                controls = controls.Where(x => x is TeleportControlData).Append(mode).ToArray();
            }
            else
            {
                if (hand.hand == Hand.left) controls = new LocomotionControlData[] { new HandedControlData() { hand = Hand.left } };
                else controls = new LocomotionControlData[] { new HandedControlData() { hand = Hand.right } };
            }

            if (hand.hand == Hand.left)
            {
                _leftControls = controls;
            }
            else if (hand.hand == Hand.right)
            {
                _rightControls = controls;
            }
        }
        else if (mode is GrabControlData grab)
        {
            // Allow no other controls on either controller
            // TODO maybe rotation is legally allowed?
            _leftControls = new LocomotionControlData[1] { mode };
            _rightControls = new LocomotionControlData[1] { mode };
        }

        UpdateControlScheme();
    }

    private static bool ValidateScheme(LocomotionControlData[] data)
    {
        bool grab = data.Any(x => x is GrabControlData);
        throw new NotImplementedException();
    }

    private static void ResetLocomotion()
    {
        SetMoveNone();
        SetRotateNone();
    }

    private static void SetMoveNone()
    {
        grabProvider.leftGrabMoveProvider.enabled = false;
        grabProvider.rightGrabMoveProvider.enabled = false;
        grabProvider.enabled = false;

        slideProvider.enabled = false;
        leftController.smoothMotionEnabled = false;
        rightController.smoothMotionEnabled = false;

        teleportProvider.enabled = false;
        leftTeleRay.enabled = false;
        rightTeleRay.enabled = false;
    }
    private static void SetRotateNone()
    {
        grabProvider.leftGrabMoveProvider.enabled = false;
        grabProvider.rightGrabMoveProvider.enabled = false;
        grabProvider.enabled = false;

        smoothTurnProvider.enabled = false;
        leftController.smoothTurnEnabled = false;
        rightController.smoothTurnEnabled = false;

        snapTurnProvider.enabled = false;
        snapTurnProvider.leftHandTurnInput.inputSourceMode = XRInputValueReader.InputSourceMode.Unused;
        snapTurnProvider.rightHandTurnInput.inputSourceMode = XRInputValueReader.InputSourceMode.Unused;
    }

    public static void SetSlideMoveData(SlideMoveData data) => throw new NotImplementedException();
    public static void SetTeleportData(TeleportControlData data) => throw new NotImplementedException();
    public static void SetSmoothTurnData(SmoothTurnData data) => throw new NotImplementedException();
    public static void SetSnapTurnData(SnapTurnData data) => throw new NotImplementedException();
    public static void SetGrabData(GrabControlData data) => throw new NotImplementedException();


    private static void UpdateControlScheme()
    {
        // Disable all controls
        ResetLocomotion();

        List<LocomotionControlData> data = new();
        data.AddRange(_leftControls);
        data.AddRange(_rightControls);

        // Reenable what's relevant
        foreach (LocomotionControlData scheme in data)
        {
            if (scheme is GrabControlData grabData)
            {
                ActivateGrab(grabData);
            }
            else if (scheme is SlideMoveData slideData)
            {
                ActivateSlideMove(slideData);
            }
            else if (scheme is TeleportControlData teleData)
            {
                ActivateTeleportControl(teleData);
            }
            else if (scheme is SmoothTurnData smoothTurnData)
            {
                ActivateSmoothTurn(smoothTurnData);
            }
            else if (scheme is SnapTurnData snapTurnData)
            {
                ActivateSnapTurn(snapTurnData);
            }
        }

        OnControlSchemeChanged.Invoke();
    }

    private static void ActivateSlideMove(SlideMoveData data)
    {
        slideProvider.enabled = true;
        if (data.hand == Hand.right)
        {
            rightController.smoothMotionEnabled = true;
        }
        else
        {
            leftController.smoothMotionEnabled = true;
        }
    }
    private static void ActivateTeleportControl(TeleportControlData data)
    {
        teleportProvider.enabled = true;
        if (data.hand == Hand.right)
        {
            rightTeleRay.enabled = true;
        }
        else
        {
            leftTeleRay.enabled = true;
        }
    }
    private static void ActivateSmoothTurn(SmoothTurnData data)
    {
        smoothTurnProvider.enabled = true;
        if (data.hand == Hand.right)
        {
            rightController.smoothTurnEnabled = true;
        }
        else
        {
            leftController.smoothTurnEnabled = true;
        }
    }
    private static void ActivateSnapTurn(SnapTurnData data)
    {
        snapTurnProvider.enabled = true;
        if (data.hand == Hand.right)
        {
            snapTurnProvider.rightHandTurnInput.inputSourceMode = XRInputValueReader.InputSourceMode.InputActionReference;
        }
        else
        {
            snapTurnProvider.leftHandTurnInput.inputSourceMode = XRInputValueReader.InputSourceMode.InputActionReference;
        }
    }

    private static void ActivateGrab(GrabControlData data)
    {
        grabProvider.leftGrabMoveProvider.enabled = true;
        grabProvider.rightGrabMoveProvider.enabled = true;
        grabProvider.enabled = true;
    }


    public enum Hand { left, right }
    public enum Mode
    {
        none = 0,
        grab,
        teleport,
        slide,
        snapTurn,
        smoothTurn
    }

    // If only C# allowed Union types :(
    public class LocomotionControlData { }
    public class GrabControlData : LocomotionControlData { }
    public class HandedControlData : LocomotionControlData { public Hand hand; }
    public class MoveData : HandedControlData { }
    public class TurnData : HandedControlData { }
    public class SlideMoveData : MoveData { }
    public class TeleportControlData : MoveData { }
    public class SmoothTurnData : TurnData { }
    public class SnapTurnData : TurnData { }
}