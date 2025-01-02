using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dial : MonoBehaviour
{
    [SerializeField] Vector2 minMaxRotation;
    [SerializeField] Transform dialHandle;
    [SerializeField] Image lightsImage;

    public UnityEvent<float> dialValueChanged;

    private void Update()
    {
        Vector3 eulerRot = dialHandle.eulerAngles;
        if (eulerRot.y < minMaxRotation.x)
        {
            eulerRot.y = minMaxRotation.x;
        }
        else if (eulerRot.y > minMaxRotation.y)
        {
            eulerRot.y = minMaxRotation.y;
        }
    }
}
