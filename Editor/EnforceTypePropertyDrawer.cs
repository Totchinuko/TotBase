using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TotBase
{
    [CustomPropertyDrawer(typeof(EnforceTypeAttribute))]
    public class EnforceTypePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnforceTypeAttribute propAttribute = attribute as EnforceTypeAttribute;
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, new GUIContent(ObjectNames.NicifyVariableName(property.name)));
            UnityEngine.Object obj = EditorGUI.ObjectField(position, property.objectReferenceValue, propAttribute.type, false);
            if (obj != null && propAttribute.type.IsAssignableFrom(obj.GetType()) && !EditorGUI.showMixedValue)
            {
                property.objectReferenceValue = obj;
            } else if(obj == null)
                property.objectReferenceValue = null;
            EditorGUI.EndProperty();
        }
    }
}
