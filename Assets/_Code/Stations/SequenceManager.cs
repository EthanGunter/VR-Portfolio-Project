using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

public class SequenceManager : SerializedMonoBehaviour
{
    public event Action<Frame> OnFramePlay;

    #region Variables

    [SerializeField] Frame[] frames = new Frame[0];
    [SerializeField] RectTransform framesIndicatorUI;

    [ShowInInspector] static Sprite visitedImage;
    [ShowInInspector] static Sprite activeImage;
    [ShowInInspector] static Sprite unvisitedImage;
    [ShowInInspector] static Sprite nauseaImage;

    [ShowInInspector, ReadOnly] public int Index { get; private set; }
    public bool HasNext => Index < frames.Length - 1;
    public bool CanSkip => Index < frames.Length - 2;
    public bool HasPrev => Index > 0;

    #endregion
    [ButtonGroup("controls")]
    public void Prev()
    {
        int oldIndex = Index;
        if (Index > 0)
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
        if (Index < frames.Length - 1)
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
        frames[old].elements.SetActive(false);

        Frame frame = frames[index];
        frame.elements.SetActive(true);
        frame.visited = true;
        Narrator.Narrate(frame.voiceover, frame.narratorPosition?.position ?? default);

        OnFramePlay?.Invoke(frame);
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