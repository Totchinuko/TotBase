using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TotBaseEditor
{
    public static class EditorTotBaseUtility
    {
        public static void RegisterEditorGlobalInput(EditorApplication.CallbackFunction callback)
        {
            System.Reflection.FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            EditorApplication.CallbackFunction value = (EditorApplication.CallbackFunction)info.GetValue(null);
            value += callback;
            info.SetValue(null, value);
        }

        public static void UnregisterEditorGlobalInput(EditorApplication.CallbackFunction callback)
        {
            System.Reflection.FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            EditorApplication.CallbackFunction value = (EditorApplication.CallbackFunction)info.GetValue(null);
            value -= callback;
            info.SetValue(null, value);
        }
    }
}