using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolarStorm.Entities
{
    public interface ITargetSensor
    {
        event Action<GameObject> OnTargetAcquired;
        event Action<GameObject> OnTargetLost;
        //IEnumerable<GameObject> GetNextTargets(int num);
    }
}