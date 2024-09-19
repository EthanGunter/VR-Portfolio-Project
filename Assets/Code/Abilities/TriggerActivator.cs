using System;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TriggerActivator : AbilityActivator
{
    #region Variables

    [SerializeField] InputActionReference leftTriggerValue, rightTriggerValue;
    [SerializeField] float activationThreshold = .9f;

    private XRGrabInteractable _grabbable;

    #endregion


    #region Unity Messages
    protected override void Awake()
    {
        base.Awake();
        previewObject.TryGetComponent(out _grabbable);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        leftTriggerValue.action.performed += LeftAction_performed;
        rightTriggerValue.action.performed += RightAction_performed;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        leftTriggerValue.action.performed -= LeftAction_performed;
        rightTriggerValue.action.performed -= RightAction_performed;
    }

    private void Update()
    {
        if (!IsChecking) return;

        IXRInteractor xRInteractor = _grabbable.GetNewestInteractorSelecting();
        if (xRInteractor?.handedness == InteractorHandedness.Left)
        {
            float value = leftTriggerValue.action.ReadValue<float>();
            if (value >= activationThreshold)
            {
                ActivationComplete_Invoke();
            }
        }
        else
        {
            float value = rightTriggerValue.action.ReadValue<float>();
            if (value >= activationThreshold)
            {
                ActivationComplete_Invoke();
            }
        }
    }

    #endregion


    #region Event Handlers

    private void RightAction_performed(InputAction.CallbackContext obj)
    {
        if (previewObject.GetComponent<XRGrabInteractable>()?.GetNewestInteractorSelecting()?.handedness == InteractorHandedness.Right)
        {
            float value = obj.action.ReadValue<float>();
            ActivationStatusChanged_Invoke(value);
            if (value >= activationThreshold)
            {
                ActivationComplete_Invoke();
            }
        }
    }

    private void LeftAction_performed(InputAction.CallbackContext obj)
    {
        if (previewObject.GetComponent<XRGrabInteractable>()?.GetNewestInteractorSelecting()?.handedness == InteractorHandedness.Left)
        {
            float value = obj.action.ReadValue<float>();
            ActivationStatusChanged_Invoke(value);
            if (value >= activationThreshold)
            {
                ActivationComplete_Invoke();
            }
        }
    }

    #endregion
}