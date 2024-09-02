using UnityEngine;
using UnityEngine.InputSystem;


public class XRHandAnimationController : MonoBehaviour
{
    [SerializeField] private InputActionProperty triggerAction;
    [SerializeField] private InputActionProperty gripAction;

    [SerializeField] private Animator animator;

    private void Awake()
    {
        if(animator == null)
        {
            if(TryGetComponent(out Animator anim))
                animator = anim;
            else
                Debug.LogError($"Failed to find animator for object {gameObject.name}");
        }
    }

    private void Update()
    {
        float triggerVal = triggerAction.action.ReadValue<float>();
        float gripVal = gripAction.action.ReadValue<float>();
        animator.SetFloat("trigger", triggerVal);
        animator.SetFloat("grip", gripVal);
    }
}
