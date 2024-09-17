using UnityEngine;

namespace SolarStorm.Entities
{
    public class EntityComponent : MonoBehaviour
    {
        protected GameEntity Entity { get; private set; }


        #region Unity Messages

        protected virtual void Awake()
        {
            if (!TryGetComponent(out GameEntity entity))
            {
                throw new MissingComponentException("Entity components cannot work without a GameEntity!");
            }
            Entity = entity;
        }

        #endregion
    }
}