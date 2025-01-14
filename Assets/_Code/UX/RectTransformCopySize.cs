using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class RectTransformCopySize : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;

    [ShowInInspector]
    private Vector2 padding;

    [ShowInInspector]
    private Vector2 minSize;
    [ShowInInspector]
    private Vector2 maxSize;

    [ShowInInspector, ReadOnly] Vector2 calculatedSize;
    [ShowInInspector, ReadOnly] Vector2 clampedSize;

    private RectTransform trans;

    private void Awake()
    {
        trans = GetComponent<RectTransform>();
    }

    private void Update()
    {
        float x = rectTransform.sizeDelta.x + padding.x * 2;
        calculatedSize.x = x;
        if (minSize.x > 0 && x < minSize.x) x = minSize.x;
        else if (maxSize.x > 0 && x > maxSize.x) x = maxSize.x;

        float y = rectTransform.sizeDelta.y + padding.y * 2;
        calculatedSize.y = y;
        if (minSize.y > 0 && y < minSize.y) y = minSize.y;
        else if (maxSize.y > 0 && y > maxSize.y) y = maxSize.y;

        clampedSize = trans.sizeDelta = new Vector2(x, y);
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!trans) trans = GetComponent<RectTransform>();
        Update();
    }
#endif
}
