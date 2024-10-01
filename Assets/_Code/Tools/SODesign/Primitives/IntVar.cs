using System;
using UnityEngine;

namespace SolarStorm.UnityToolkit
{
    [CreateAssetMenu(menuName = "Shared Data/Primitives/Int")]
    public class IntVar : ScriptableObject
    {
        public int Value;
        public void SetValue(int value)
        {
            Value = value;
        }
    }

    [Serializable]
    public class IntRef
    {
        [SerializeField]
        private bool useInstanced = true;
        [SerializeField]
        private IntVar shared;
        [SerializeField]
        private int instanced;

        public int Value
        {
            get { return useInstanced ? instanced : shared.Value; }
            set
            {
                if (useInstanced) instanced = value;
                else shared.Value = value;
            }
        }

        public IntRef(int value)
        {
            instanced = value;
            useInstanced = true;
        }

        public static implicit operator int(IntRef fRef) => fRef.Value;
        public static implicit operator IntRef(int val) => new IntRef(val);
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}