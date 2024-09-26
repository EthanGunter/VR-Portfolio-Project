using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Ability set")]
public class AbilitySet : ScriptableObject, IReadOnlyCollection<AbilityData>
{
    [SerializeField] AbilityData[] _abilities;

    public AbilityData CreateCopyOfIndex(int index)
    {
        return Instantiate(_abilities[index]);
    }
    public IEnumerable<AbilityData> CreateRuntimeCopy()
    {
        AbilityData[] abilityDatas = new AbilityData[_abilities.Length];
        for (int i = 0; i < abilityDatas.Length; i++)
        {
            abilityDatas[i] = Instantiate(_abilities[i]);
        }
        return abilityDatas;
    }

    public int Count => ((IReadOnlyCollection<AbilityData>)_abilities).Count;

    public IEnumerator<AbilityData> GetEnumerator()
    {
        return ((IEnumerable<AbilityData>)_abilities).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _abilities.GetEnumerator();
    }
}