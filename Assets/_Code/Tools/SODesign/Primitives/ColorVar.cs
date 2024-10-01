using System;
using UnityEngine;

namespace SolarStorm.UnityToolkit
{
    [CreateAssetMenu(menuName = "Shared Data/Primitives/Color")]
    public class ColorVar : ScriptableObject
    {
        public Color Value;
        public void SetValue(Color value)
        {
            Value = value;
        }
    }

    [Serializable]
    public class ColorRef
    {
        [SerializeField]
        private bool useInstanced = true;
        [SerializeField]
        private ColorVar shared;
        [SerializeField]
        private Color instanced;

        public Color Value
        {
            get { return useInstanced ? instanced : shared.Value; }
            set
            {
                if (useInstanced) instanced = value;
                else shared.Value = value;
            }
        }

        public ColorRef(Color value)
        {
            instanced = value;
            useInstanced = true;
        }

        public static implicit operator Color(ColorRef fRef) => fRef.Value;
        public static implicit operator ColorRef(Color val) => new ColorRef(val);
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}