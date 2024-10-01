using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarStorm.Entities
{
    public interface ITargetSensor
    {
        HealthComponent GetTarget();
        
        // TODO should this be required for all target sensors? Or should there be an IMultiTargetSensor?
        // IEnumerable<GameObject> GetTargets(int num);
    }
}