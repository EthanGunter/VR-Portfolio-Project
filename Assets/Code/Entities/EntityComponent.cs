using UnityEngine;

namespace SolarStorm.Entities
{
    [RequireComponent(typeof(GameEntity))]
    public class EntityComponent : MonoBehaviour
    {
        protected GameEntity Entity { get; private set; }


        #region Unity Messages

        protected virtual void Awake()
        {
            Entity = GetComponent<GameEntity>();
        }

        #endregion
    }
}