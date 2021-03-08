using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;
using System.Text;

namespace TotBaseEditor.DataTable
{
    public class DataTableRow
    {
        public const float ROWHEIGHT = 30f;

        public DataTableRow(int headersCount, ScriptableObject row, string key)
        {
            sizes = new float[headersCount];
            this.row = row;
            this.key = key;
        }

        private float[] sizes;
        private List<Func<ScriptableObject,string>> columnDefinitions = new List<Func<ScriptableObject, string>>(); 
        public ScriptableObject row;
        private float totalSize;
        public string key;
        private float keySize;
        private bool selected;

        public event EventHandler Clicked;

        public void Draw()
        {
            Rect line = EditorGUILayout.BeginHorizontal(GUILayout.Width(totalSize), GUILayout.Height(ROWHEIGHT));
            GUI.DrawTexture(line, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, false, 1f, new Color(.3f, .3f, .3F), 0, 0);
            Rect cell;

            cell = EditorGUILayout.BeginHorizontal(GUILayout.Width(keySize), GUILayout.Height(ROWHEIGHT));
            GUI.DrawTexture(cell, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, false, 1f, new Color(.15f, .15f, .15F), 0, 0);
            Color col = GUI.color;
            if (selected)
                GUI.color = new Color(1f, .5f, 0f);
            EditorGUILayout.LabelField(key, GUILayout.Width(keySize));
            GUI.color = col;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5f);

            string text = "";
            for (int i = 0; i < sizes.Length; i++)
            {
                cell = EditorGUILayout.BeginHorizontal(GUILayout.Width(sizes[i]), GUILayout.Height(ROWHEIGHT));
                GUI.DrawTexture(cell, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, false, 1f, new Color(.2f, .2f, .2F), 0, 0);
                text = "N/A";
                if(row != null)
                    text = columnDefinitions[i]?.Invoke(row);
                EditorGUILayout.LabelField(text, GUILayout.Width(sizes[i]));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(5f);
            }
            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.MouseUp && line.Contains(Event.current.mousePosition))
            {
                OnClicked();
            }
        }

        public void AppendColumn(Func<ScriptableObject,string> function)
        {
            columnDefinitions.Add(function);
        }

        public string GetSearchString()
        {
            if(row == null)
                 return "";
            StringBuilder builder = new StringBuilder();
            foreach(Func<ScriptableObject, string> column in columnDefinitions)
            {
                builder.Append(column.Invoke(row));
            }
            return builder.ToString();
        }

        public ICollection<string> GetFields()
        {
            Type t = row.GetType();
            FieldInfo[] infos = t.GetFields();
            List<string> fields = new List<string>(infos.Length);

            foreach(FieldInfo i in infos)
            {
                object v = i.GetValue(row);
                if (i.FieldType.IsEnum && i.FieldType.GetCustomAttributes(typeof(FlagsAttribute), false).Any())
                    fields.Add(((int)v).ToString());
                else if (v != null)
                    fields.Add(v.ToString());
                else
                    fields.Add("null");
            }

            return fields;
        }

        protected virtual void OnClicked()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }

        public void SetSelected(bool selected)
        {
            this.selected = selected;
        }

        public void SetHeadersSizes(ref float[] sizes, float total, float keySize)
        {
            this.sizes = sizes;
            totalSize = total;
            this.keySize = keySize;
        }
    }

}
