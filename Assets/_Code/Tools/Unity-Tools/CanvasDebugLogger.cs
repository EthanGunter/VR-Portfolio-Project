using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CanvasDebugLogger : MonoBehaviour
{
    #region Variables

    [SerializeField] TextMeshProUGUI display;
    private RectTransform textRect;

    [SerializeField] int maxLength = 1000;

    [SerializeField] Color logColor = Color.white;
    [SerializeField] Color warningColor = Color.yellow;
    [SerializeField] Color errorColor = Color.red;

    public bool ShowInfo { get; set; } = true;
    public bool ShowWarnings { get; set; } = false;
    public bool ShowErrors { get; set; } = true;

    ActionSortedDictionary<string, Message> logs = new ActionSortedDictionary<string, Message>((a, b) =>
    {
        if (a.Value.label != null && b.Value.label == null) return 1;
        else if (b.Value.label != null && a.Value.label == null) return -1;
        else if (a.Value.lastLogged > b.Value.lastLogged) return 1;
        else if (a.Value.lastLogged < b.Value.lastLogged) return -1;
        else return 0;
    });

    #endregion


    #region Unity Messages

    private void Awake()
    {
        textRect = display.GetComponent<RectTransform>();
        Application.logMessageReceived += HandleLog;
    }
    private void Start()
    {
        if (!display) { Debug.LogError("UIDebugLogger needs a text component to write to"); return; }
    }
    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    #endregion


    private void HandleLog(string condition, string stackTrace, LogType type)
    {
        string[] split = condition.Split(':');
        string key = split[0];
        string value = split.Length > 1 ? condition : key;

        ColorizeLogString(ref value, type);

        Message msg = new Message()
        {
            label = split.Length > 1 ? key : null,
            text = value,
            type = type,
            lastLogged = Time.time
        };

        if (logs.ContainsKey(key))
        {
            // TODO Add some method to determine if a recurring message is still being sent
            logs[key] = msg;
        }
        else
        {
            logs.Add(key, msg);
        }

        if (logs.Count > maxLength)
        {
            logs.RemoveFirst();
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        string output = "";
        foreach (var logItem in logs.GetSortedItems(false))
        {
            Message msg = logItem.Value;
            if (msg.text.Length > 0)
            {
                output += msg.text + "(" + msg.lastLogged + ")\n";
            }
        }

        this.display.text = output;
        textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, this.display.textBounds.size.y);
    }

    private void ColorizeLogString(ref string value, LogType type)
    {
        string hexCode = "#ffffff";
        switch (type)
        {
            case LogType.Log:
                hexCode = ColorUtility.ToHtmlStringRGB(logColor);
                break;
            case LogType.Assert:
            case LogType.Exception:
            case LogType.Error:
                hexCode = ColorUtility.ToHtmlStringRGBA(errorColor);
                break;
            case LogType.Warning:
                hexCode = ColorUtility.ToHtmlStringRGBA(warningColor);
                break;
        }

        value = $"<color=#{hexCode}>{value}</color>";
    }

    private struct Message
    {
        public string label;
        public string text;
        public LogType type;
        public float lastLogged;
    }
}