using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class EasyInputHandler : MonoBehaviour
{
    #region Variables

    [SerializeField] InputActionReference inputAction;
    [SerializeField] UnityEvent handlers;


    #endregion


    #region Unity Messages

    private void Awake()
    {
        if (inputAction == null) Destroy(this);
        InputSystem.onDeviceChange += onDeviceChange;
    }

    private void OnEnable()
    {
        inputAction.action.Enable();
        inputAction.action.performed += Invoke;
    }

    private void OnDisable()
    {
        inputAction.action.Disable();
        inputAction.action.performed -= Invoke;
    }

    #endregion


    private void onDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Disconnected: OnDisable(); break;
            case InputDeviceChange.Reconnected: OnEnable(); break;
        }
    }

    private void Invoke(InputAction.CallbackContext obj)
    {
        handlers.Invoke();
    }
}