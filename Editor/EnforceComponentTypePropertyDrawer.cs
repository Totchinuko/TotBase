using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TotBase
{
    [CustomPropertyDrawer(typeof(EnforceComponentTypeAttribute))]
    public class EnforceComponentTypePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnforceComponentTypeAttribute propAttribute = attribute as EnforceComponentTypeAttribute;
            EditorGUI.BeginProperty(position, label, property);
            Component comp = (Component)property.objectReferenceValue;
            GameObject obj = default;
            if(comp != null)
                obj = comp.gameObject;
            obj = EditorGUI.ObjectField(position, new GUIContent($"Component ({propAttribute.type.Name})"), obj, typeof(GameObject), true) as GameObject;
            if(obj == null)
                property.objectReferenceValue = null;
            else {
                comp = obj.GetComponent(propAttribute.type) as Component;
                if(comp != null)
                    property.objectReferenceValue = comp;
            }
            EditorGUI.EndProperty();
        }
    }
}
