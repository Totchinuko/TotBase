using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using TotBase;

namespace TotBaseEditor
{
    public abstract class DataTableInspector<T> : Editor where T : struct
    {
        private bool _showRaw;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open with DataTable Editor"))
            {
                EditorWindow window = GetEditorWindow();

                if (!window) return;
                if (target.GetType().GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDataTable<T>)))
                    return;
                IDataTable<T> table = (IDataTable<T>)target;
                ((DataTableEditorWindow<T>)window).LoadTable(table);
            }

            if (GUILayout.Button("Show Raw"))
            {
                _showRaw = true;
            }

            if (_showRaw)
            {
                DrawDefaultInspector();
            }
        }

        public abstract DataTableEditorWindow<T> GetEditorWindow();
    }
}
