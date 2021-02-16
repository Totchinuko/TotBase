using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace TotBaseEditor
{
    public class DataTableResizableArea
    {
        public Color color;
        public float areaSize;
        private bool vertical;
        private float pos;
        private bool resize = false;
        private Vector2 scroll;

        public event EventHandler Repainted;

        public DataTableResizableArea(Color color, float areaSize, bool vertical, float pos = 0f)
        {
            this.areaSize = areaSize;
            this.color = color;
            this.vertical = vertical;
            this.pos = pos;
        }

        public void Draw(Rect container, Action<Vector2, float> action)
        {
            scroll = EditorGUILayout.BeginScrollView(scroll, vertical ? GUILayout.Width(areaSize) : GUILayout.Height(areaSize));
            action(scroll, areaSize);
            EditorGUILayout.EndScrollView();
            GUILayout.Space(10f);
            Rect cursorChangeRect = new Rect(vertical ? areaSize + pos : 0f, vertical ? 0f : areaSize + pos, vertical ? 5f : container.width, vertical ? container.height : 5f);
            GUI.DrawTexture(cursorChangeRect, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, false, 1f, Color.grey, 0f, 0f);
            EditorGUIUtility.AddCursorRect(cursorChangeRect, MouseCursor.ResizeVertical);

            if (Event.current.type == EventType.MouseDown && cursorChangeRect.Contains(Event.current.mousePosition))
                resize = true;
            if (resize)
            {
                areaSize = vertical ? Event.current.mousePosition.x - pos : Event.current.mousePosition.y - pos;
                OnRepainted();
            }

            if (Event.current.rawType == EventType.MouseUp)
                resize = false;
        }

        protected virtual void OnRepainted()
        {
            Repainted?.Invoke(this, EventArgs.Empty);
        }
    }
}
