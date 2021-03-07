using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TotBase.DataTable;

namespace TotBaseEditor.DataTable
{
    [CustomPropertyDrawer(typeof(DataTablePointer))]
    public class DataTablePointerInspector : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            SerializedProperty dataTable = property.FindPropertyRelative("_datatable");
            SerializedProperty key = property.FindPropertyRelative("_key");

            Rect main = new Rect(position.x, position.y, position.width, position.height);
            Rect popup = new Rect(position.x, position.y, position.width - 50f, position.height);
            Rect button = new Rect(popup.x + popup.width, position.y, 50f, position.height);


            if (dataTable.objectReferenceValue == null || !(dataTable.objectReferenceValue is IAnonymousDataTable))
            {
                if (!(dataTable.objectReferenceValue is IAnonymousDataTable))
                    dataTable.objectReferenceValue = null;
                EditorGUI.PropertyField(main, dataTable, new GUIContent(""));
            }
            else
            {
                IAnonymousDataTable table = (IAnonymousDataTable)dataTable.objectReferenceValue;
                List<string> keys = new List<string>(table.GetKeys());
                int index = keys.IndexOf(key.stringValue);
                int newIndex = EditorGUI.Popup(popup, index, keys.ToArray());
                if (index != newIndex)
                {
                    key.stringValue = keys[newIndex];
                }
                if (GUI.Button(button, "Clear"))
                {
                    dataTable.objectReferenceValue = null;
                    key.stringValue = null;
                }
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public static bool DrawStandalone(DataTablePointer pointer)
        {
            IAnonymousDataTable dt = (IAnonymousDataTable)pointer._datatable;
            ScriptableObject datatable = pointer._datatable;
            string key = pointer._key;

            if (dt == null)
            {
                datatable = (ScriptableObject)EditorGUILayout.ObjectField("", datatable, typeof(ScriptableObject), false);
                if (datatable != pointer._datatable)
                {
                    if (typeof(IAnonymousDataTable).IsAssignableFrom(datatable.GetType()))
                    {
                        pointer._datatable = datatable;
                        return true;
                    }
                }
            }
            else
            {
                List<string> keys = new List<string>(dt.GetKeys());
                int index = keys.IndexOf(key);

                int newIndex = EditorGUILayout.Popup(index, keys.ToArray());
                if (newIndex != index)
                {
                    pointer._key = keys[newIndex];
                    return true;
                }
                if (GUILayout.Button("Change table", GUILayout.Width(100f)))
                {
                    pointer._datatable = null;
                    pointer._key = null;
                    return true;
                }
            }

            return false;
        }
    }
}
