
using SolarStorm.UnityToolkit;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Splines;
using UnityEngine.Splines.Interpolators;

public class DuelDeckView : MonoBehaviour
{
    #region Variables

    [SerializeField] DuelDeck _deck;
    [SerializeField] CardSocket socketPrefab;

    [SerializeField] float cardSocketWidth = .08f;
    [SerializeField] SplineContainer cardPath;

    [SerializeField]
    [Tooltip("The amount of time it takes each socket to stop moving after a reorganization")]
    float socketAnimationTime = .2f;
    [SerializeField] EasingFunction socketLerpFunction;

    private readonly Dictionary<AbilityData, CardSocket> _cardSockets = new Dictionary<AbilityData, CardSocket>();
    private readonly Dictionary<CardSocket, PosRot> _socketTransformHolds = new Dictionary<CardSocket, PosRot>();
    private ObjectPool<CardSocket> _socketPool;

    #endregion


    #region Unity Messages

    private void Awake()
    {
        _socketPool = new ObjectPool<CardSocket>(CreateSocket, GetSocket, ReleaseSocket, DestroySocket);
        //_deck.Hand.OnOrderChange += Hand_OnChange;
        _deck.Hand.OnCardsAdded += Hand_OnCardsAdded;
        _deck.Hand.OnCardsRemoved += Hand_OnCardsRemoved;
    }

    private void OnDestroy()
    {
        //_deck.Hand.OnOrderChange -= Hand_OnChange;
        _deck.Hand.OnCardsAdded -= Hand_OnCardsAdded;
        _deck.Hand.OnCardsRemoved -= Hand_OnCardsRemoved;
    }


    private void Update()
    {
        AnimateCardSockets();
    }

    #endregion


    #region Event Handlers

    private void Hand_OnCardsAdded(IEnumerable<AbilityData> added)
    {
        foreach (AbilityData ability in added)
        {
            if (_cardSockets.ContainsKey(ability))
            {
                Debug.LogError($"Attempted to create duplicate socket for card {ability.name}", this);
                continue;
            }

            CardSocket socket = _socketPool.Get();

            _cardSockets.Add(ability, socket);
            _socketTransformHolds.Add(socket, new PosRot());

            socket.SeatAbility(ability, true);
        }
    }

    private void Hand_OnCardsRemoved(IEnumerable<AbilityData> removed)
    {
        foreach (AbilityData ability in removed)
        {
            if (!_cardSockets.TryGetValue(ability, out CardSocket socket))
            {
                Debug.LogError($"Attempted to remove nonexistant socket for card {ability.name}", this);
                continue;
            }

            _cardSockets.Remove(ability);
            _socketTransformHolds.Remove(socket);
            _socketPool.Release(socket);
        }
    }

    #endregion


    #region Utility

    private void AnimateCardSockets()
    {
        float length = cardPath.Spline.GetLength();
        float cardsLength = cardSocketWidth * _cardSockets.Count;
        float start = (length - cardsLength) / 2;
        for (int i = 0; i < _cardSockets.Count; i++)
        {
            CardSocket socket = _cardSockets[_deck.Hand[i]];

            float t = i * cardSocketWidth / length + start;

            if (!SplineUtility.Evaluate(cardPath.Spline, t, out float3 pos, out float3 tangent, out float3 upVec))
            {
                Debug.LogError("Duel Deck failed card path spline evaluation", this);
            }


            PosRot hold = _socketTransformHolds[socket];

            // Set the card's target position
            Vector3 targetPos = pos;

            // Rotate the card to face outward
            Quaternion targetRot = Quaternion.LookRotation(upVec);

            // Animate them
            float percent = EasingFunctions.Ease((_socketTransformHolds[socket].remainingAnimationTime -= Time.deltaTime) / socketAnimationTime, socketLerpFunction);

            socket.transform.SetLocalPositionAndRotation(
                Vector3.Lerp(hold.pos, targetPos, percent),
                Quaternion.Lerp(hold.rot, targetRot, percent)
            );
        }
    }


    #region Socket Pool

    /// <summary>
    /// Allocates an object for the pool to manipulate
    /// </summary>
    private CardSocket CreateSocket()
    {
        return Instantiate(socketPrefab, transform);
    }

    /// <summary>
    /// Prepares the queued object so it can be placed in the world
    /// </summary>
    private void GetSocket(CardSocket socket)
    {
        socket.gameObject.SetActive(true);
    }

    /// <summary>
    /// Disables the pooled object so it can return to the queue
    /// </summary>
    private void ReleaseSocket(CardSocket socket)
    {
        socket.RemoveActiveAbility();
        socket.gameObject.SetActive(false);
    }

    /// <summary>
    /// Deallocate the pooled object
    /// </summary>
    private void DestroySocket(CardSocket socket)
    {
        Destroy(socket);
    }

    #endregion


    private class PosRot
    {
        public Vector3 pos = Vector3.zero;
        public Quaternion rot = Quaternion.identity;
        public float remainingAnimationTime = 0;
    }

    #endregion
}
