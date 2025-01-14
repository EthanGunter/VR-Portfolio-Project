using Sirenix.OdinInspector;
using System;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class RectTransformSizeConstraint : MonoBehaviour
{
    #region Variables

    [ShowInInspector] AxisMap[] constraints = new AxisMap[0];

    public RectTransform Rect
    {
        get
        {
            if (rect == null)
                rect = GetComponent<RectTransform>();
            return rect;
        }
        set { rect = value; }
    }
    private RectTransform rect;


    #endregion


    #region Unity Messages

    private void LateUpdate()
    {
        UpdateSize();
    }

    #endregion


    private void UpdateSize()
    {
        if (rect == null) return;

        float newX = Rect.sizeDelta.x, newY = Rect.sizeDelta.y;

        foreach (var constraint in constraints)
        {
            if(constraint.targetRect == null)
            {
                Debug.LogError($"Constraint {constraint.localAxis} => {constraint.targetAxis} is missing a target reference", this);
                continue;
            }
            switch (constraint.localAxis)
            {
                case Axis.X:
                    switch (constraint.targetAxis)
                    {
                        case Axis.X:
                            newX = constraint.targetRect.sizeDelta.x;
                            break;
                    }
                    break;
            }
        }

        rect.sizeDelta = new Vector2(newX, newY);
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateSize();
    }
#endif

    [Serializable]
    public struct AxisMap
    {
        public Axis localAxis;
        public Axis targetAxis;
        public RectTransform targetRect;
    }
    public enum Axis { X, Y, Z }
}