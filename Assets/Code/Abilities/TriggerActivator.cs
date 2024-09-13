using System;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TriggerActivator : AbilityActivator
{
    #region Variables

    [SerializeField] InputActionReference leftTriggerValue, rightTriggerValue;
    [SerializeField] float activationThreshold = .9f;

    #endregion


    #region Unity Messages

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

    //private void Update()
    //{
    //    //if (active)
    //    //{
    //    if (previewObject.HeldBy?.handedness == InteractorHandedness.None
    //        &&
    //        (
    //        leftTriggerValue.action.ReadValue<float>() >= activationThreshold
    //        || rightTriggerValue.action.ReadValue<float>() >= activationThreshold
    //        ))
    //    {
    //        activationComplete?.Invoke();
    //    }
    //    else if (previewObject.HeldBy?.handedness == InteractorHandedness.Left && leftTriggerValue.action.ReadValue<float>() >= activationThreshold
    //        || previewObject.HeldBy?.handedness == InteractorHandedness.Right && rightTriggerValue.action.ReadValue<float>() >= activationThreshold)
    //    {
    //        activationComplete?.Invoke();
    //    }
    //    //}
    //}

    #endregion


    #region Event Handlers

    private void RightAction_performed(InputAction.CallbackContext obj)
    {
        if (previewObject.HeldBy?.handedness == InteractorHandedness.Right)
        {
            float value = obj.action.ReadValue<float>();
            activationStatusChanged?.Invoke(value);
            if (value >= activationThreshold)
            {
                activationComplete?.Invoke();
            }
        }
    }

    private void LeftAction_performed(InputAction.CallbackContext obj)
    {
        if (previewObject.HeldBy?.handedness == InteractorHandedness.Left)
        {
            float value = obj.action.ReadValue<float>();
            activationStatusChanged?.Invoke(value);
            if (value >= activationThreshold)
            {
                activationComplete?.Invoke();
            }
        }
    }

    #endregion
}