using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace SolarStorm.UnityToolkit
{
    public static class MonoBehaviourExtensions
    {
        private static readonly Vector3 FAR_AWAY = new Vector3(0, (float)1.0E+10, 0);
        /// <summary>
        /// This method of destruction will trigger OnTrigger and OnCollision Exit unity 0messages
        /// </summary>
        public static async void Destroy(this GameObject obj)
        {
            obj.transform.position = FAR_AWAY;
            await Task.Delay(100);
            UnityEngine.Object.Destroy(obj);
        }

        public static List<Transform> GetChildren(this Transform transform, bool recursive = false)
        {
            List<Transform> children = new List<Transform>();

            foreach (Transform child in transform)
            {
                children.Add(child);
                if (recursive)
                {
                    children.AddRange(child.GetChildren(true));
                }
            }

            return children;
        }

        /// <summary>
        /// Returns the first instance of a component in the ancestor heirarchy
        /// </summary>
        public static T GetComponentInAncestors<T>(this Transform transform, bool includeSelf = false) where T : Component
        {
            T component = null;
            Transform target = includeSelf ? transform : transform.parent;
            do
            {
                component = target.GetComponent<T>();
                target = transform.parent;
            }
            while (target != null && component == null);

            return component;
        }

        /// <summary>
        /// Returns all instances of a component type in the ancestor heirarchy
        /// </summary>
        public static List<T> GetComponentsInAncestors<T>(this Transform transform, bool includeSelf = false) where T : Component
        {
            T component = null;
            List<T> components = new();
            Transform target = includeSelf ? transform : transform.parent;
            do
            {
                component = target.GetComponent<T>();
                if (component != null) components.Add(component);

                target = target.parent;
            }
            while (target != null && component == null);

            return components;
        }
    }
    public static class ComponentExtensions
    {
        public static T GetCopyFrom<T>(this T comp, T other) where T : Component
        {
            Type type = comp.GetType();
            Type othersType = other.GetType();
            if (type != othersType)
            {
                Debug.LogError($"The type \"{type.AssemblyQualifiedName}\" of \"{comp}\" does not match the type \"{othersType.AssemblyQualifiedName}\" of \"{other}\"!");
                return null;
            }

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default;
            PropertyInfo[] pinfos = type.GetProperties(flags);

            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch
                    {
                        /*
                         * In case of NotImplementedException being thrown.
                         * For some reason specifying that exception didn't seem to catch it,
                         * so I didn't catch anything specific.
                         */
                    }
                }
            }

            FieldInfo[] finfos = type.GetFields(flags);

            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }
            return comp as T;
        }
    }
}
