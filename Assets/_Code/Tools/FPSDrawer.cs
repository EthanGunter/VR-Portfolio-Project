using System;
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FPSDrawer : MonoBehaviour
{
    [SerializeField] int targetFrameRate = 72;
    [SerializeField] Color belowTargetFPSColor = Color.red;

    private float frameRate;
    private TextMeshProUGUI text;
    Color defaultColor;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        defaultColor = text.color;
    }

    private void Start()
    {
        StartCoroutine(DrawFPS());
    }

    private void Update()
    {
        frameRate = Mathf.Floor(1 / Time.unscaledDeltaTime);
    }

    private IEnumerator DrawFPS()
    {
        while (true)
        {
            if (frameRate >= targetFrameRate)
            {
                text.color = defaultColor;
                text.text = $"FPS: {frameRate}";
            }
            else
            {
                text.color = belowTargetFPSColor;
                text.text = $"FPS: {frameRate}";
            }
            yield return new WaitForEndOfFrame();
        }
    }
}