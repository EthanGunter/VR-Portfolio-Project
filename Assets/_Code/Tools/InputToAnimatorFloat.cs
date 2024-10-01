using SolarStorm.UnityToolkit;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputToAnimatorFloat : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] StringRef animatorParameterName;
    [SerializeField] InputActionProperty inputAction;

    private void Update()
    {
        animator.SetFloat(animatorParameterName, inputAction.action.ReadValue<float>());
    }
}
