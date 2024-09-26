using UnityEngine;
using UnityEngine.Events;

public class TimedCardActivator : AbilityActivator
{
    [SerializeField] float timeToActivate;

    private float _time;


    protected override void StartActivationCheck()
    {
        base.StartActivationCheck();
        _time = 0;
    }

    private void Update()
    {
        if (IsChecking)
        {
            if (_time > timeToActivate)
            {
                ActivationComplete_Invoke();
                StopActivationCheck();
            }
            else
            {
                _time += Time.deltaTime;
                ActivationStatus = _time / timeToActivate;
            }
        }
    }
}