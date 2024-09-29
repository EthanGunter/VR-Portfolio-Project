using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WristMenu : MonoBehaviour
{
    [SerializeField] ContextMenuData menuData;

    [SerializeField] Camera playerCamera;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeTime = .2f;
    [SerializeField] float onThreshold = .9f;

    [SerializeField] Transform contentParent;
    [SerializeField] Button buttonPrefab;

    private Dictionary<string, Button> buttons = new();
    private Dictionary<string, Action> buttonActions = new();

    private Tween fadeTween;


    private void Awake()
    {
        menuData.OnItemAdded += OnButtonAdded;
        menuData.OnItemRemoved += OnButtonRemoved;
    }
    private void OnDestroy()
    {
        menuData.OnItemAdded -= OnButtonAdded;
        menuData.OnItemRemoved -= OnButtonRemoved;
    }

    private void Update()
    {
        float facingPlayerDot = Vector3.Dot(playerCamera.transform.forward, canvasGroup.transform.forward);
        if (facingPlayerDot > onThreshold)
        {
            DOTween.Kill(fadeTween);
            canvasGroup.gameObject.SetActive(true);
            fadeTween = canvasGroup.DOFade(1, fadeTime)
                .SetId(fadeTween);
        }
        else
        {
            DOTween.Kill(fadeTween);
            fadeTween = canvasGroup.DOFade(0, fadeTime)
                .SetId(fadeTween)
                .OnComplete(() =>
                {
                    canvasGroup.gameObject.SetActive(false);
                });
        }
    }


    public void OnButtonAdded(string displayText, Action logic, int index)
    {
        Button button = Instantiate(buttonPrefab, contentParent);
        button.onClick.AddListener(() => ButtonClicked(displayText));
        button.transform.SetSiblingIndex(index);

        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text == null) throw new ArgumentException($"{buttonPrefab.name} does not have a TextMeshPro component in its hierachy to write text to");
        text.text = displayText;

        buttons.Add(displayText, button);
        buttonActions.Add(displayText, logic);
    }

    public void OnButtonRemoved(string displayText)
    {
        if (buttons.TryGetValue(displayText, out var button))
        {
            Destroy(button.gameObject);
            buttons.Remove(displayText);
        }

        buttonActions.Remove(displayText);
    }

    private void ButtonClicked(string displayName)
    {
        if (buttonActions.TryGetValue(displayName, out var action))
        {
            action.Invoke();
        }
        else
        {
            Debug.Log($"Failed to find menu action {displayName}", this);
        }
    }
}
