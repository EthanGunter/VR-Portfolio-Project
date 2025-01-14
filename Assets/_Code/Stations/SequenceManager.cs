using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SequenceManager : SerializedMonoBehaviour
{
    public event Action<Frame> OnFramePlay;

    #region Variables

    [SerializeField] Frame[] frames = new Frame[0];
    [SerializeField] RectTransform framesIndicatorUI;

    [SerializeField, AssetsOnly] RectTransform visitedPrefab;
    [SerializeField, AssetsOnly] RectTransform activePrefab;
    [SerializeField, AssetsOnly] RectTransform unvisitedPrefab;
    [SerializeField, AssetsOnly] RectTransform nauseaIndicatorPrefab;

    [ShowInInspector, ReadOnly] public int Index { get; private set; } = -1;
    [ShowInInspector] public bool AlwaysSkip { get; set; } = false;
    public bool HasNext => Index < frames.Length - 1;
    public bool CanSkip => Index < frames.Length - 2;
    public bool HasPrev => Index > 0;

    #endregion


    #region Unity Messages

    private void Awake()
    {
        foreach (var frame in frames)
        {
            frame.elements.SetActive(false);
        }

        // TODO Spawn??
        Index = 0;
        PlayFrame(-1, 0);
    }

    #endregion


    [ButtonGroup("controls")]
    public void Prev()
    {
        int oldIndex = Index;
        if (AlwaysSkip && Index > 0 && frames[Index - 1].nauseaRisk)
        {
            Index -= 2;
            PlayFrame(oldIndex, Index);
        }
        else if (Index > 0)
        {
            Index--;
            PlayFrame(oldIndex, Index);
        }
        //throw new NotImplementedException();
    }
    [ButtonGroup("controls")]
    public void Next()
    {
        int oldIndex = Index;
        if (AlwaysSkip && Index < frames.Length - 1 && frames[Index + 1].nauseaRisk) Skip();
        else if (Index < frames.Length - 1)
        {
            Index++;
            PlayFrame(oldIndex, Index);
        }
        //throw new NotImplementedException();
    }

    [ButtonGroup("controls")]
    public void Skip()
    {
        int oldIndex = Index;
        if (Index < frames.Length - 2)
        {
            Index += 2;
            PlayFrame(oldIndex, Index);
        }
        //throw new NotImplementedException();
    }


    private void PlayFrame(int old, int index)
    {
        if (old != -1) frames[old].elements.SetActive(false);

        Frame frame = frames[index];
        frame.elements.SetActive(true);
        frames[index].visited = true;
        Narrator.Narrate(frame.voiceover, frame.narratorPosition != null ? frame.narratorPosition.position : default);

        UpdateIndicatorUI();

        OnFramePlay?.Invoke(frame);
    }

    private void UpdateIndicatorUI()
    {
        if (framesIndicatorUI == null) return;

        // This isn't optimal, but it's less likely to break than managing things by hand
        // And if I've learned anything over the years, it's don't preoptimize!
        foreach (Transform child in framesIndicatorUI)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < frames.Length; i++)
        {
            Frame frame = frames[i];
            RectTransform icon = null;
            if (i == Index)
                icon = Instantiate(activePrefab);
            else if (frame.visited)
                icon = Instantiate(visitedPrefab);
            else
                icon = Instantiate(unvisitedPrefab);

            if (icon != null && frame.nauseaRisk)
            {
                RectTransform nauseaIndicator = Instantiate(nauseaIndicatorPrefab);
                nauseaIndicator.SetParent(icon, false);
            }

            icon.SetParent(framesIndicatorUI, false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(framesIndicatorUI);
    }

    [Serializable]
    public struct Frame
    {
        [Required]
        public GameObject elements;
        [Required]
        public AudioClip voiceover;
        public Transform narratorPosition;
        public bool nauseaRisk;
        [ReadOnly] public bool visited;
    }
}