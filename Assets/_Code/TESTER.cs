using EasyButtons;
using UnityEngine;

public class TESTER : MonoBehaviour
{
    #region Variables
    [SerializeField] StationSpawnAnimator anim;
    #endregion


    [Button]
    private void In()
    {
        anim.AnimateIn();
    }
    [Button]
    private void Out()
    {
        anim.AnimateOut();
    }
}