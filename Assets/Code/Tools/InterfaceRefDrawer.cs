using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(InterfaceReference), true)]
public class InterfaceReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Get the _viewObject property
        SerializedProperty viewObjectProperty = property.FindPropertyRelative("_interfaceObject");

        // Draw the object field
        EditorGUI.ObjectField(position, viewObjectProperty, label);

        // Validate the assigned object
        if (viewObjectProperty.objectReferenceValue != null)
        {
            var targetType = fieldInfo.FieldType;
            if (targetType.IsGenericType)
            {
                targetType = targetType.GetGenericArguments()[0];
            }

            if (!targetType.IsInstanceOfType(viewObjectProperty.objectReferenceValue))
            {
                viewObjectProperty.objectReferenceValue = null;
            }
        }

        EditorGUI.EndProperty();
    }
}
