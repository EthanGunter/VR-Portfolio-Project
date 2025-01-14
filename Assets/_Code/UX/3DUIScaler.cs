using Sirenix.OdinInspector;
using SolarStorm.UnityToolkit;
using UnityEngine;
using UnityEngine.UI;

public class Sync3DUI : MonoBehaviour/*, ICanvasElement Couldn't get to work in <10min */
{
    #region Variables

    [ShowInInspector, ReadOnly] RectTransform rect;
    [ShowInInspector, ReadOnly] RectTransform root;

    #endregion


    #region Unity Messages

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        Canvas canvas = transform.GetComponentInAncestors<Canvas>();
        root = canvas?.GetComponent<RectTransform>();

        if (root == null) throw new System.Exception("Failed to find canvas");
    }

    private void LateUpdate()
    {
        transform.localScale = new Vector3(rect.sizeDelta.x, rect.sizeDelta.y, transform.localScale.z);
    }

    #endregion

#if UNITY_EDITOR
    [Sirenix.OdinInspector.Button]
    private void InspectorUpdate()
    {
        LateUpdate();
    }
#endif
}