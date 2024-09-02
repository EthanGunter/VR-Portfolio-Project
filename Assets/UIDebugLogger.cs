using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDebugLogger : MonoBehaviour
{
    List<string> logs = new List<string>();
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] int maxLength = 10000;

    private void OnEnable()
    {
        if (!text) { gameObject.SetActive(false); Debug.LogError("UIDebugLogger needs a text component to write to"); return; }
        Application.logMessageReceived += HandleLog;
    }
    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string condition, string stackTrace, LogType type)
    {
        logs.Add(condition);

        DisplayText();
    }

    private void DisplayText()
    {
        string output = "";
        foreach (var logItem in logs)
        {
            output += logItem + "\n";
        }

        if(output.Length > maxLength)
            output = output.Substring(maxLength);

        text.text = output;
    }
}
