using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SolarStorm.Entities
{
    public interface ITurretWeapon
    {
        /// <summary>
        /// Passes the barrel index shot from
        /// </summary>
        event Action<int> OnShoot;
        /// <summary>
        /// Attacks the given target
        /// </summary>
        void Attack(GameObject target, TurretLevelData data);
    }

    public interface IWeaponAsync
    {
        /// <summary>
        /// Attempts to attack the given target
        /// </summary>
        /// <returns>true if the attack is eventually successful</returns>
        Task<bool> Attack(GameObject target);
    }
}