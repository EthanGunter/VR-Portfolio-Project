using UnityEngine;

public class Ability : AbilityComponent
{
    #region Variables

    #endregion


    #region Unity Messages

    #endregion

    public async override Awaitable Show(bool skipAnimation = false)
    {
        gameObject.SetActive(true);
    }

    public async override Awaitable Hide(bool skipAnimation = false)
    {
        gameObject.SetActive(false);
    }
}