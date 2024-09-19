using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(DissolvingViewComponent))]
public abstract class AbilityActivator : MonoBehaviour
{
    public event Action ActivationComplete;
    public event Action<float> ActivationStatusChanged;

    //protected AbilityData ability;
    public bool IsChecking { get; private set; }
    protected float ActivationStatus
    {
        get => _activationStatus; set
        {
            if (_activationStatus != value) ActivationStatusChanged?.Invoke(value);
            _activationStatus = value;
        }
    }
    private float _activationStatus;

    protected AbilityViewComponent previewObject;


    #region Unity Messages

    protected virtual void Awake()
    {
        previewObject = GetComponent<DissolvingViewComponent>();
    }
    protected virtual void OnEnable()
    {
        StartActivationCheck();
    }
    protected virtual void OnDisable()
    {
        StopActivationCheck();
    }

    #endregion


    /// <returns>A value from 0 to 1 expressing how "activated" a card is. Will usually be a binary response.</returns>
    public float GetActivationStatus()
    {
        return _activationStatus;
    }

    protected virtual void StartActivationCheck()
    {
        IsChecking = true;
    }
    protected virtual void StopActivationCheck()
    {
        IsChecking = false;
    }

    protected void ActivationComplete_Invoke()
    {
        IsChecking = false;
        ActivationComplete?.Invoke();
    }

    protected virtual void ActivationStatusChanged_Invoke(float value)
    {
        ActivationStatusChanged?.Invoke(value);
    }
}
