using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using System.IO;
using System.Text;

namespace TotBaseEditor
{
    public class PowerCreateMenuWindow : EditorWindow
    {
        private bool alt;
        private string search;
        private List<PCMMenuElement> elements = new List<PCMMenuElement>(50);
        private List<PCMMenuElement> filtered;
        private Vector2 scroll;
        private bool registered;
        private bool focus;

        [MenuItem("Window/TotBase/Power Creation Menu")]
        public static void OpenWindow() {
            PowerCreateMenuWindow win = EditorWindow.GetWindow<PowerCreateMenuWindow>("Creation Menu");
            win.OnEnable();
        }

        private void OnEnable() {
            if(!registered) {
                EditorUtils.RegisterEditorGlobalInput(OnMainInputPressed);
                registered = true;
            }
        }

        private void OnDisable() {
            EditorUtils.UnregisterEditorGlobalInput(OnMainInputPressed);
            registered = false;
        } 

        private void OnMainInputPressed()
        {
            if(Event.current.keyCode == KeyCode.LeftControl && Event.current.type == EventType.KeyDown)
                alt = true;
            else if(Event.current.keyCode == KeyCode.LeftControl && Event.current.type == EventType.KeyUp)
                alt = false;
            else if(Event.current.keyCode == KeyCode.Q && Event.current.type == EventType.KeyDown && alt)
            {
                Focus(); 
                focus = true;
                EditorGUI.FocusTextInControl("TotPowerSearch");
                Event.current.Use();
            }
        }

        private void OnGUI() {
            if(elements.Count == 0)
                SearchForMenus();

            EditorGUILayout.BeginVertical();
            GUI.SetNextControlName("TotPowerSearch");
            if(Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyDown && filtered.Count == 1 && GUI.GetNameOfFocusedControl() == "TotPowerSearch"){
                filtered[0].Execute();
                Event.current.Use();
            }
                
            EditorGUI.BeginChangeCheck();
            search = EditorGUILayout.TextField(search);
            if(focus)
            {
                EditorGUI.FocusTextInControl("TotPowerSearch");
                focus = false;
            }
            if(EditorGUI.EndChangeCheck())
                RefreshFiltered();
            


            scroll = EditorGUILayout.BeginScrollView(scroll);
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            for (int i = 0; i < filtered.Count; i++)
            {
                if(GUILayout.Button(filtered[i].name))
                    filtered[i].Execute();
            }
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void SearchForMenus() {
            elements.Clear();

            elements.Add(new PCMCustomMenu("csharp Script", CreateCSharpScript));
            elements.Add(new PCMCustomMenu("SurfShader", CreateSurfShader));

            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.GetCustomAttributes(typeof(CreateAssetMenuAttribute), false).Length > 0)
                .ToList()
                .ForEach(x => elements.Add(new PCMScriptableObject(x)));
            
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .SelectMany(x => x.GetMethods())
                .Where(x => x.IsStatic && x.GetCustomAttributes(typeof(MenuItem), false).Length > 0)
                .Where(x => ((MenuItem)x.GetCustomAttributes(typeof(MenuItem), false)[0]).menuItem.StartsWith("Assets/Create"))
                .ToList()
                .ForEach(x => elements.Add(new PCMMenuItem(x)));     

            filtered = new List<PCMMenuElement>(elements.Count);
            RefreshFiltered();
        }

        private void RefreshFiltered() {
            if(string.IsNullOrEmpty(search))
            {
                filtered = elements.ToList();
                return;
            }
            filtered.Clear();
            for (int i = 0; i < elements.Count; i++)
            {
                if(elements[i].name.ToLower().Contains(search.ToLower()))
                    filtered.Add(elements[i]);
            }
        }

        private void CreateCSharpScript() {
            string folder = EditorUtils.getActiveFolderPath();
            string filePath = AssetDatabase.GenerateUniqueAssetPath (folder + "/NewScript.cs");
            File.WriteAllText(filePath, "");
            AssetDatabase.Refresh();
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(filePath, typeof(MonoScript));
            Selection.SetActiveObjectWithContext(obj, null);
        }

        private void CreateSurfShader() {
            StringBuilder code = new StringBuilder();
            code.AppendLine("BEGIN_OPTIONS");
            code.AppendLine("");
            code.AppendLine("END_OPTIONS");
            code.AppendLine("BEGIN_PROPERTIES");
            code.AppendLine("");
            code.AppendLine("END_PROPERTIES");
            code.AppendLine("BEGIN_CBUFFER");
            code.AppendLine("");
            code.AppendLine("END_CBUFFER");
            code.AppendLine("BEGIN_CODE");
            code.AppendLine("");
            code.AppendLine("END_CODE");
            string folder = EditorUtils.getActiveFolderPath();
            string filePath = AssetDatabase.GenerateUniqueAssetPath (folder + "/NewShader.surfshader");
            File.WriteAllText(filePath, code.ToString());
            AssetDatabase.Refresh();
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(filePath, typeof(MonoScript));
            Selection.SetActiveObjectWithContext(obj, null);
        }
    }
}