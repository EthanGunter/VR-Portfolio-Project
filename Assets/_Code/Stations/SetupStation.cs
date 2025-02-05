using SolarStorm.UnityToolkit;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using static LocomotionController;

public class SetupStation : Station
{
    #region Variables

    [SerializeField] PokeButton setHeightBtn;

    // LEFT CONTROLLER
    [SerializeField] PokeButton staticBtn_left;

    // Movement
    [SerializeField] PokeButton slideBtn_left;
    [SerializeField] PokeButton teleportBtn_left;

    // Rotation
    [SerializeField] PokeButton snapTurnBtn_left;
    [SerializeField] PokeButton smoothTurnBtn_left;


    // RIGHT CONTROLLER    
    [SerializeField] PokeButton staticBtn_right;

    // Movement
    [SerializeField] PokeButton slideBtn_right;
    [SerializeField] PokeButton teleportBtn_right;

    // Rotation
    [SerializeField] PokeButton snapTurnBtn_right;
    [SerializeField] PokeButton smoothTurnBtn_right;

    #endregion


    #region Unity Messages

    private void Awake()
    {
        OnControlSchemeChanged += OnControlsChanged;

        setHeightBtn.ButtonPressed.AddListener(() =>
        {
            Player.RequestHeightUpdate();
        });

        staticBtn_left.ButtonPressed.AddListener(() =>
        {
            SetControl(new HandedControlData() { hand = Hand.left });
        });
        slideBtn_left.ButtonPressed.AddListener(() =>
        {
            SetControl(new SlideMoveData() { hand = Hand.left });
        });
        teleportBtn_left.ButtonPressed.AddListener(() =>
        {
            SetControl(new TeleportControlData() { hand = Hand.left });
        });

        snapTurnBtn_left.ButtonPressed.AddListener(() =>
        {
            SetControl(new SnapTurnData() { hand = Hand.left });
        });
        smoothTurnBtn_left.ButtonPressed.AddListener(() =>
        {
            SetControl(new SmoothTurnData() { hand = Hand.left });
        });

        staticBtn_right.ButtonPressed.AddListener(() =>
        {
            SetControl(new HandedControlData() { hand = Hand.right });
        });
        slideBtn_right.ButtonPressed.AddListener(() =>
        {
            SetControl(new SlideMoveData() { hand = Hand.right });
        });
        teleportBtn_right.ButtonPressed.AddListener(() =>
        {
            SetControl(new TeleportControlData() { hand = Hand.right });
        });

        snapTurnBtn_right.ButtonPressed.AddListener(() =>
        {
            SetControl(new SnapTurnData() { hand = Hand.right });
        });
        smoothTurnBtn_right.ButtonPressed.AddListener(() =>
        {
            SetControl(new SmoothTurnData() { hand = Hand.right });
        });
    }

    private void OnDestroy()
    {
        OnControlSchemeChanged -= OnControlsChanged;
    }

    #endregion


    private void OnControlsChanged()
    {
        DisableButtons();
        LocomotionControlData[] controls = GetControlScheme();
        foreach (var item in controls)
        {
            if (item is SlideMoveData slideMoveData)
            {
                if (slideMoveData.hand == Hand.right) slideBtn_right.IsActive = true;
                else slideBtn_left.IsActive = true;
            }
            else if (item is TeleportControlData teleportControlData)
            {
                if (teleportControlData.hand == Hand.right) teleportBtn_right.IsActive = true;
                else teleportBtn_left.IsActive = true;
            }
            else if (item is SmoothTurnData smoothTurnData)
            {
                if (smoothTurnData.hand == Hand.right) smoothTurnBtn_right.IsActive = true;
                else smoothTurnBtn_left.IsActive = true;
            }
            else if (item is SnapTurnData snapTurnData)
            {
                if (snapTurnData.hand == Hand.right) snapTurnBtn_right.IsActive = true;
                else snapTurnBtn_left.IsActive = true;
            }
            else if (item is HandedControlData data)
            {
                if (data.hand == Hand.right)
                {
                    staticBtn_right.IsActive = true;
                }
                else
                {
                    staticBtn_left.IsActive = true;
                }

            }
        }
    }

    private void DisableButtons()
    {
        staticBtn_left.IsActive = false;
        slideBtn_left.IsActive = false;
        teleportBtn_left.IsActive = false;
        snapTurnBtn_left.IsActive = false;
        smoothTurnBtn_left.IsActive = false;
        staticBtn_right.IsActive = false;
        slideBtn_right.IsActive = false;
        teleportBtn_right.IsActive = false;
        snapTurnBtn_right.IsActive = false;
        smoothTurnBtn_right.IsActive = false;
    }
}