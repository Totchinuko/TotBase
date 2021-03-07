using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace TotBaseEditor
{
    public class DataTableHeader
    {
        public DataTableHeader(string name, float size = 200f)
        {
            this.name = name;
            this.size = size;
        }

        public string name;
        public float size;
        private bool selected;
        private bool resize;
        private float mousePos;

        public event EventHandler Clicked;
        public event EventHandler Repainted;
        public event EventHandler Resized;

        public void Draw()
        {
            Rect Area = EditorGUILayout.BeginHorizontal(GUILayout.Width(size), GUILayout.Height(DataTableRow.ROWHEIGHT));
            GUI.DrawTexture(Area, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, false, 1f, new Color(.1f, .1f, .1F), 0, 0);
            EditorGUILayout.LabelField(name, GUILayout.Width(size));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5f);
            Rect slider = new Rect(Area.x + Area.width, Area.y, 5f, Area.height);
            GUI.DrawTexture(slider, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, false, 1f, Color.grey, 0f, 0f);
            EditorGUIUtility.AddCursorRect(slider, MouseCursor.ResizeHorizontal);

            if (Event.current.type == EventType.MouseDown && slider.Contains(Event.current.mousePosition))
            {
                resize = true;
                mousePos = Event.current.mousePosition.x;
            }
            else if (Event.current.type == EventType.MouseUp && Area.Contains(Event.current.mousePosition))
            {
                OnClicked();
            }
            if (resize)
            {
                if (Event.current.mousePosition.x - mousePos != 0f)
                {
                    size += Event.current.mousePosition.x - mousePos;
                    size = Math.Max(size, 10);
                    mousePos = Event.current.mousePosition.x;
                    OnResized();
                    OnRepainted();
                }
            }
            if (Event.current.rawType == EventType.MouseUp)
                resize = false;
        }

        public void SetSelected(bool selected)
        {
            this.selected = selected;
        }

        protected virtual void OnClicked()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnRepainted()
        {
            Repainted?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnResized()
        {
            Resized?.Invoke(this, EventArgs.Empty);
        }
    }
}
