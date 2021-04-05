using System;
using UnityEngine;
using UnityEditor;

namespace TotBase
{
    [CustomPropertyDrawer(typeof(AssetOnlyAttribute))]
    public class AssetOnlyAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, new GUIContent(ObjectNames.NicifyVariableName(property.name)));
            if(property.propertyType != SerializedPropertyType.ObjectReference)
                EditorGUI.LabelField(position, "Field type not a UnityEngine.Object");
            else
                property.objectReferenceValue = EditorGUI.ObjectField(position, property.objectReferenceValue, fieldInfo.FieldType, false);
            EditorGUI.EndProperty();
        }
    }
}