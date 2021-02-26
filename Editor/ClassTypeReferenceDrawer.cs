using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TotBase;
using System.Linq;

[CustomPropertyDrawer(typeof(ClassTypeReference))]
public class ClassTypeReferenceDrawer : PropertyDrawer
{
    private List<string> entries = new List<string>();
    private List<Type> tEntries = new List<Type>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        Rect main = new Rect(position.x, position.y, position.width, position.height);


        ClassTypeParentAttribute attr = null;
        object[] attributes = fieldInfo.GetCustomAttributes(typeof(ClassTypeParentAttribute), false);
        if(attributes.Length > 0)
        {
            attr = (ClassTypeParentAttribute)attributes.First();

            List<Type> types = new List<Type>();
            PopulateEntries(attr);
            int index = GetIndexOfType(property);

            int newIndex = EditorGUI.Popup(main, index, entries.ToArray());

            if (index != newIndex)
            {
                Type t = tEntries[newIndex];
                property.FindPropertyRelative("_classRef").stringValue = ClassTypeReference.GetClassRef(t);
            }
        }
        else
        {
            EditorGUI.LabelField(main, "Please use ClassTypeParentAttribute");
        }
            
        
        

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    private int GetIndexOfType(SerializedProperty p)
    {
        string name = Type.GetType(p.FindPropertyRelative("_classRef").stringValue)?.ToString();
        for (int i = 0; i < entries.Count; i++)
            if (entries[i] == name)
                return i;

        return 0;
    }

    private void PopulateEntries(ClassTypeParentAttribute attr)
    {
        if (entries.Count == 0)
        {
            if (attr == null)
            {
                foreach (Type t in TypeHelper.NonSystemicTypes)
                    if(t.IsClass)
                        entries.Add(t.ToString());
            }
            else
            {
                Type type = attr.Type;
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p) && ((p.IsAbstract && attr.allowAbstract) || !p.IsAbstract) && p.IsClass);
                entries.Add("None");
                tEntries.Add(null);
                foreach (Type t in types)
                {
                    if (t.IsClass)
                    {
                        entries.Add(t.ToString());
                        tEntries.Add(t);
                    }                        
                }
                    
            }
        }
    }
}
