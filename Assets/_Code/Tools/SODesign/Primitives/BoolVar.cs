using System;
using UnityEngine;

namespace SolarStorm.UnityToolkit
{
    [CreateAssetMenu(menuName = "Shared Data/Primitives/Bool")]
    public class BoolVar : ScriptableObject
    {
        public bool Value;
        public void SetValue(bool value)
        {
            Value = value;
        }
    }

    [Serializable]
    public class BoolRef
    {
        [SerializeField]
        private bool useConstant = true;
        [SerializeField]
        private BoolVar variable;
        [SerializeField]
        private bool constant;

        public bool Value
        {
            get { return useConstant ? constant : variable.Value; }
            set
            {
                if (useConstant) throw new InvalidOperationException("Cannot set a constant FloatRef");
                else variable.Value = value;
            }
        }

        public BoolRef(bool value)
        {
            constant = value;
            useConstant = true;
        }
        public static implicit operator bool(BoolRef bRef) => bRef.Value;
        public static implicit operator BoolRef(bool balue) => new BoolRef(balue);
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
