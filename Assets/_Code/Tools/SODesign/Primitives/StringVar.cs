using System;
using UnityEngine;

namespace SolarStorm.UnityToolkit
{
    [CreateAssetMenu(menuName = "Shared Data/Primitives/String")]
    public class StringVar : ScriptableObject
    {
        public string Value;
        public void SetValue(string value)
        {
            Value = value;
        }
    }

    [Serializable]
    public class StringRef
    {
        [SerializeField]
        private bool useConstant = true;
        [SerializeField]
        private StringVar variable;
        [SerializeField]
        private string constant;

        public string Value
        {
            get { return useConstant ? constant : variable.Value; }
            set
            {
                if (useConstant) throw new InvalidOperationException("Cannot set a constant StringRef");
                else variable.Value = value;
            }
        }

        public StringRef(string value)
        {
            constant = value;
            useConstant = true;
        }
        public static implicit operator string(StringRef sRef) => sRef.Value;
        public static implicit operator StringRef(string val) => new StringRef(val);

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}