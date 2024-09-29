using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class RectTransformCopySize : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Vector2 padding;

    private RectTransform trans;

    private void Awake()
    {
        trans = GetComponent<RectTransform>();
    }

    private void Update()
    {
        trans.sizeDelta = new Vector2(rectTransform.sizeDelta.x + padding.x * 2, rectTransform.sizeDelta.y + padding.y * 2);
    }


#if UNITY_EDITOR
    [EasyButtons.Button]
    private void InspectorUpdate()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(rectTransform.sizeDelta.x + padding.x * 2, rectTransform.sizeDelta.y + padding.y * 2);
    }
#endif
}
