using UnityEngine;
using UnityEngine.Events;

public class CountdownEvent : MonoBehaviour
{
    #region Variables

    [SerializeField] float waitTime;

    public UnityEvent action;

    private float _waitTime;

    #endregion


    #region Unity Messages

    private void Awake()
    {
        _waitTime = waitTime;
    }

    private void Update()
    {
        _waitTime -= Time.deltaTime;
        if (_waitTime < 0)
        {
            action.Invoke();
            _waitTime = waitTime;
            enabled = false;
        }
    }

    #endregion
}