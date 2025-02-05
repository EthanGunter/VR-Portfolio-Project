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

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Transform rotationParent;
    [SerializeField] float fadeTime = .2f;
    [SerializeField, Tooltip("0 only checks the angle of the watch, 1 only checks the angle of the headset")] float facingWeight = .8f;
    [SerializeField] float onThreshold = .8f;

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
        Vector3 headToWatchVec = (transform.position - Player.Head.transform.position).normalized;
        Vector3 watchToHeadVec = (Player.Head.transform.position - transform.position).normalized;

        float playerFacingDot = Vector3.Dot(headToWatchVec, Player.Head.transform.forward);
        float facingPlayerDot = Vector3.Dot(watchToHeadVec, -canvasGroup.transform.forward); // Unity canvases are backward for some reason
        if (facingPlayerDot < 0) facingPlayerDot = -1;

        if ((facingPlayerDot * (1 - facingWeight) + playerFacingDot * facingWeight) > onThreshold)
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
