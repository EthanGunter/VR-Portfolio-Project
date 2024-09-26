using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolarStorm.Entities
{
    public interface ITargetSensor
    {
        event Action<GameEntity> OnTargetAcquired;
        event Action<GameEntity> OnTargetLost;
        //IEnumerable<GameEntity> GetNextTargets(int num);
    }
}