using SolarStorm.UnityToolkit;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WristMenuController : MonoBehaviour
{
    #region Variables

    [SerializeField] RectTransform tabButtonPrefab;
    [SerializeField] RectTransform contentContainer;
    [SerializeField] RectTransform tabContainer;


    private Dictionary<string, RectTransform> contexts = new();
    private Dictionary<string, Button> contextTabs = new();

    private string activeContext;

    #endregion


    private void Awake()
    {
        if (contentContainer.childCount > 0 && tabContainer.childCount > 0)
        {
            RectTransform content = contentContainer.GetChild(0)?.GetComponent<RectTransform>();
            content?.gameObject.SetActive(false);
            Button btn = tabContainer.GetChild(0)?.GetComponent<Button>();
            if (content && btn)
            {
                AddContextMenu("default", btn, content);
            }
        }
    }


    public void RemoveContextMenu(string key)
    {
        if (contexts.TryGetValue(key, out var context))
        {
            Destroy(context.gameObject);
            contexts.Remove(key);
        }
        if (contextTabs.TryGetValue(key, out var tab))
        {
            Destroy(tab.gameObject);
            contextTabs.Remove(key);
        }
    }

    public void AddContextMenu(string key, string displayName, RectTransform content)
    {
        AddContextMenu(
            key,
            CreateTextTabButton(displayName),
            content
            );
    }
    public void AddContextMenu(string key, Sprite displayIcon, RectTransform content)
    {
        AddContextMenu(
            key,
            CreateImageTabButton(displayIcon),
            content);
    }
    public void AddContextMenu(string key, RectTransform tabPrefab, RectTransform content)
    {
        if (tabPrefab.GetComponent<Button>() == null)
            throw new ArgumentException($"{tabPrefab.name} cannot be used as a tabPrefab button because it has no button component");
        Button btn = Instantiate(tabPrefab).GetComponent<Button>();
        btn.gameObject.SetActive(true);

        AddContextMenu(
            key,
            btn,
            content);
    }

    private void AddContextMenu(string key, Button tabButton, RectTransform content)
    {
        content = Instantiate(content);

        contexts.Add(key, content);
        contextTabs.Add(key, tabButton);

        // TODO Put context menu in content panel
        content.SetParent(contentContainer, false);
        content.gameObject.SetActive(true);

        // TODO Put tab in tab-group
        tabButton.transform.SetParent(tabContainer, false);

        // TODO Configure tab button to trigger switching
        // Do we need to deregister if it's getting destroyed?
        tabButton.onClick.AddListener(() => { SwitchContext(key); });
    }

    private void SwitchContext(string key)
    {
        // Deactivate old context
        if (activeContext != null)
        {
            contexts[activeContext].gameObject.SetActive(false);
            contextTabs[activeContext].interactable = true;
        }

        // Activate new one
        contexts[key].gameObject.SetActive(true);
        contextTabs[key].interactable = false;

        activeContext = key;
    }



    private Button CreateTextTabButton(string displayText)
    {
        RectTransform clone = Instantiate(tabButtonPrefab);
        Button btn = clone.GetComponent<Button>();
        if (!btn)
        {
            Destroy(clone);
            throw new ArgumentException($"{tabButtonPrefab.name} is an invalid tab button. It does not have a button component.");
        }

        TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (!tmp)
        {
            Destroy(clone);
            throw new ArgumentException($"{tabButtonPrefab.name} is an invalid text tab button. There is no TextMeshPro component in the button hierarchy.");
        }
        tmp.text = displayText;

        btn.GetComponentInChildren<Image>().gameObject.SetActive(false);

        return btn;
    }
    private Button CreateImageTabButton(Sprite image)
    {
        RectTransform clone = Instantiate(tabButtonPrefab);
        Button btn = clone.GetComponent<Button>();
        if (!btn)
        {
            Destroy(clone);
            throw new ArgumentException($"{tabButtonPrefab.name} is an invalid tab button. It does not have a button.");
        }

        Image img = btn.GetComponentInChildren<Image>();
        if (!img)
        {
            Destroy(clone);
            throw new ArgumentException($"{tabButtonPrefab.name} is an invalid image tab button. There is no Image component in the button hierarchy.");
        }
        img.sprite = image;

        btn.GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);

        return btn;
    }
}