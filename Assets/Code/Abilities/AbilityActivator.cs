using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Preview))]
public abstract class AbilityActivator : MonoBehaviour
{
    public UnityEvent activationComplete;
    public UnityEvent<float> activationStatusChanged;

    //protected AbilityData ability;
    protected bool active;
    protected float ActivationStatus
    {
        get => _activationStatus; set
        {
            if (_activationStatus != value) activationStatusChanged?.Invoke(value);
            _activationStatus = value;
        }
    }
    private float _activationStatus;

    protected Preview previewObject;


    #region Unity Messages

    protected virtual void Awake()
    {
        //ability.OnStateChange += HandleAbilityStateChange;
        previewObject = GetComponent<Preview>();
    }
    protected virtual void OnEnable()
    {
        StartActivationCheck();
    }
    protected virtual void OnDisable()
    {
        StopActivationCheck();
    }
    protected virtual void OnDestroy()
    {
        //ability.OnStateChange -= HandleAbilityStateChange;
    }

    #endregion


    /// <returns>A value from 0 to 1 expressing how "activated" a card is. Will usually be a binary response.</returns>
    public float GetActivationStatus()
    {
        return _activationStatus;
    }

    private void HandleAbilityStateChange(AbilityData ability)
    {
        if (ability.State == AbilityState.Card)
        {
            StopActivationCheck();
        }
        else if (ability.State == AbilityState.Preview)
        {
            StartActivationCheck();
        }
    }

    protected virtual void StartActivationCheck()
    {
        active = true;
    }
    protected virtual void StopActivationCheck()
    {
        active = false;
    }
}
