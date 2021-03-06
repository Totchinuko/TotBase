using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TotBase
{
    [CustomPropertyDrawer(typeof(DoubleFloat))]
    public class DoubleFloatPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            object[] obj = fieldInfo.GetCustomAttributes(typeof(DoubleFloatRangeAttribute), false);
            DoubleFloatRangeAttribute range = obj.Length > 0 ? (DoubleFloatRangeAttribute)obj[0] : null;            
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, new GUIContent(ObjectNames.NicifyVariableName(property.name)));
            SerializedProperty min = property.FindPropertyRelative("min");
            SerializedProperty max = property.FindPropertyRelative("max");
            if(range == null)
            {
                position.width /= 2;
                min.floatValue = EditorGUI.FloatField(position, min.floatValue);
                position.x += position.width;
                max.floatValue = EditorGUI.FloatField(position, max.floatValue);
            }
            else
            {
                float minf = min.floatValue;
                float maxf = max.floatValue;
                Rect before = new Rect(position.x, position.y, 40f, position.height);
                Rect after = new Rect(position.x + position.width - 40f, position.y, 40f, position.height);
                position.x += 45f;
                position.width -= 90f;
                EditorGUI.LabelField(before, minf.ToString("#0.00"));
                EditorGUI.LabelField(after, maxf.ToString("#0.00"));
                EditorGUI.MinMaxSlider(position, ref minf, ref maxf, range.min, range.max);                
                min.floatValue = minf;
                max.floatValue = maxf;
            }
            
            EditorGUI.EndProperty();
        }
    }
}
