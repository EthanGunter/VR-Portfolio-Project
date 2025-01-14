using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WristUI : MonoBehaviour
{
    #region Inspector Fields

    [SerializeField] ContextMenuData menuData;

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Transform rotationParent;
    [SerializeField] float fadeTime = .2f;
    [SerializeField] float onThreshold = .9f;

    #endregion


    public bool Open { get; private set; }

    private Tween fadeTween;


    #region Unity Messages

    private void Start()
    {
        CalculateOpacity();
    }

    private void Update()
    {
        CalculateOpacity();
        if (Open)
        {
            OrientToView();
        }
    }

    #endregion


    #region Utility

    /// <summary>
    /// Makes the menu opaque if it's being looked at, and transparent if not
    /// </summary>
    private void CalculateOpacity()
    {
        float facingPlayerDot = Vector3.Dot(Player.Head.transform.forward, canvasGroup.transform.forward);
        if (facingPlayerDot > onThreshold)
        {
            // If Facing the player
            if (!Open)
            {
                DOTween.Kill(fadeTween);
                //canvasGroup.gameObject.SetActive(true);
                fadeTween = canvasGroup.DOFade(1, (1 - canvasGroup.alpha) * fadeTime)
                    .SetId(fadeTween);
                Open = true;
            }
        }
        else
        {
            // If turned away from player
            if (Open)
            {
                DOTween.Kill(fadeTween);
                fadeTween = canvasGroup.DOFade(0, canvasGroup.alpha * fadeTime)
                    .SetId(fadeTween);
                //.OnComplete(() =>
                //{
                //    canvasGroup.gameObject.SetActive(false);
                //});
                Open = false;
            }
        }
    }

    /// <summary>
    /// Orients the canvas so it appears upright to the user
    /// </summary>
    private void OrientToView()
    {
        rotationParent.transform.rotation = Quaternion.LookRotation(transform.position - Player.Head.position, Vector3.up);
        Vector3 locRot = rotationParent.localEulerAngles;
        rotationParent.localEulerAngles = new Vector3(0, 0, locRot.z);
    }

    #endregion
}
