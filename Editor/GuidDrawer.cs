using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using TotBase;

namespace TotBaseEditor
{
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    public class GuidDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // if their no initial Guid, generate one
            if(property.FindPropertyRelative("suid").stringValue == Guid.Empty.ToString())
                property.FindPropertyRelative("suid").stringValue = Guid.NewGuid().ToString();

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            Rect guid = new Rect(position.x, position.y, 150, position.height);
            Rect button = new Rect(position.x + 155, position.y, 65, position.height);

            EditorGUI.LabelField(guid, property.FindPropertyRelative("suid").stringValue.Split('-')[0] + "...");
            if(GUI.Button(button, "Refresh"))
            {
                property.FindPropertyRelative("suid").stringValue = Guid.NewGuid().ToString();                                        
            }

            EditorGUI.EndProperty();
        }
    }
}
