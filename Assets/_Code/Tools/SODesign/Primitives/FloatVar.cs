using System;
using UnityEngine;

namespace SolarStorm.UnityToolkit
{
    [CreateAssetMenu(menuName = "Shared Data/Primitives/Float")]
    public class FloatVar : ScriptableObject
    {
        public float Value;
        public void SetValue(float value)
        {
            Value = value;
        }
    }

    [Serializable]
    public class FloatRef
    {
        [SerializeField]
        private bool useInstanced = true;
        [SerializeField]
        private FloatVar shared;
        [SerializeField]
        private float instanced;

        public float Value
        {
            get { return useInstanced ? instanced : shared.Value; }
            set
            {
                if (useInstanced) instanced = value;
                else shared.Value = value;
            }
        }

        public FloatRef(float value)
        {
            instanced = value;
            useInstanced = true;
        }

        public static implicit operator float(FloatRef fRef) => fRef.Value;
        public static implicit operator FloatRef(float val) => new FloatRef(val);
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}