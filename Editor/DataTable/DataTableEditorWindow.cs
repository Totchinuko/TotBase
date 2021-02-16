using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using TotBase;

namespace TotBaseEditor
{
    public struct DebugTest
    {
        public string test1;
        public int test2;
        public float test3;
    }

    public abstract class DataTableEditorWindow<T> : EditorWindow where T : struct
    {
        private IDataTable<T> datatable;
        private Vector2 editorScroll;

        private float[] sizes;
        private float totalSizes;

        private DataTableHeader keyHeader;
        private List<DataTableHeader> headers = new List<DataTableHeader>();
        private List<DataTableRow> rows = new List<DataTableRow>();
        private DataTableResizableArea tableArea;
        private DataTableHeader selectedHeader;
        private FieldInfo selectedField;
        private DataTableRow selectedRow;
        private string idbox = "";
        private string search;
        private List<DataTableRow> filtered = new List<DataTableRow>();

        private bool addAction;
        private bool renameAction;
        private bool deleteAction;

        public void LoadTable(IDataTable<T> table)
        {
            datatable = table;

            headers.Clear();
            FieldInfo[] fields = table.GetStructType().GetFields();
            foreach (FieldInfo info in fields)
            {
                DataTableHeader h = new DataTableHeader(info.Name);
                h.Clicked += OnHeaderClicked;
                h.Repainted += OnRepaint;
                h.Resized += OnResized;
                headers.Add(h);
            }
            keyHeader = new DataTableHeader("Key", 80f);
            keyHeader.Repainted += OnRepaint;
            keyHeader.Resized += OnResized;
            keyHeader.Clicked += OnHeaderClicked;

            rows.Clear();
            table.ForEachEntries((string key, T value) =>
            {
                DataTableRow r = new DataTableRow(headers.Count, value, key);
                r.Clicked += OnRowClicked;
                rows.Add(r);

            });
            OnSearchUpdated();
            RefreshRowSizes();
            Repaint();
        }

        private void OnRowClicked(object sender, EventArgs e)
        {
            if (selectedRow != null)
                selectedRow.SetSelected(false);
            selectedRow = (DataTableRow)sender;
            selectedRow.SetSelected(true);
            idbox = selectedRow.key;
            Repaint();
        }

        private void OnResized(object sender, EventArgs e)
        {
            RefreshRowSizes();
        }

        void OnEnable()
        {
            tableArea = new DataTableResizableArea(Color.grey, this.position.height / 2, false, 30f);
            tableArea.Repainted += OnRepaint;
        }

        private void OnRepaint(object sender, EventArgs e)
        {
            Repaint();
        }

        private void OnHeaderClicked(object sender, EventArgs e)
        {
            DataTableHeader header = ((DataTableHeader)sender);
            if (selectedHeader != header)
            {
                if (selectedHeader != null)
                {
                    selectedHeader.SetSelected(false);
                    selectedHeader = null;
                }
                selectedHeader = header;
                selectedHeader.SetSelected(true);
                selectedField = datatable.GetStructType().GetField(selectedHeader.name);
                if (header == keyHeader)
                    rows.Sort(SortKeys);
                else
                    rows.Sort(SortSelected);
            }
            else
                rows.Reverse();
            OnSearchUpdated();
            Repaint();
        }

        private void OnSearchUpdated()
        {
            if(string.IsNullOrEmpty(search))
            {
                filtered = new List<DataTableRow>(rows);
                Repaint();
                return;
            }

            filtered = new List<DataTableRow>();
            foreach (DataTableRow row in rows)
                if (row.GetFields().Any(x => x.Contains(search)))
                    filtered.Add(row);
            Repaint();
        }

        private int SortSelected(DataTableRow x, DataTableRow y)
        {
            string A = selectedField.GetValue(x.row)?.ToString();
            string B = selectedField.GetValue(y.row)?.ToString();
            return A.CompareTo(B);
        }

        private int SortKeys(DataTableRow x, DataTableRow y)
        {
            return x.key.CompareTo(y.key);
        }

        private void RefreshRowSizes()
        {
            float total = keyHeader.size + 5f;
            sizes = new float[headers.Count];
            for (int i = 0; i < headers.Count; i++)
            {
                sizes[i] = headers[i].size;
                total += headers[i].size + 5f;
            }

            foreach (DataTableRow row in rows)
                row.SetHeadersSizes(ref sizes, total, keyHeader.size);
        }

        void OnGUI()
        {
            AddAction();
            RenameAction();
            DeleteAction();

            if (datatable == null)
            {
                EditorGUILayout.LabelField("No table loaded.");
                return;
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal(GUILayout.Height(30f));
            string nsearch = EditorGUILayout.TextField("Search", search);
            if(nsearch != search)
            {
                search = nsearch;
                OnSearchUpdated();
            }
            EditorGUILayout.EndHorizontal();

            tableArea.Draw(this.position, (Vector2 scroll, float size) =>
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(totalSizes), GUILayout.Height(DataTableRow.ROWHEIGHT));
                keyHeader.Draw();
                headers.ForEach(x => x.Draw());
                EditorGUILayout.EndHorizontal();
                int skipAmount = Math.Max(0, (int)Math.Floor(scroll.y / DataTableRow.ROWHEIGHT) - 1);
                int drawAmount = (int)Mathf.Floor(size / DataTableRow.ROWHEIGHT) + 2;
                int ignoreAmount = Math.Max(0, rows.Count - drawAmount - skipAmount);
                GUILayout.Space(DataTableRow.ROWHEIGHT * skipAmount);
                for (int i = skipAmount; i < filtered.Count && i < drawAmount + skipAmount; i++)
                    filtered[i].Draw();
                GUILayout.Space(DataTableRow.ROWHEIGHT * ignoreAmount);
            });

            GUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal();
            DrawRowHeader();
            EditorGUILayout.EndHorizontal();

            if (rows.Count > 0 && selectedRow != null)
            {
                GUILayout.Space(10f);
                editorScroll = EditorGUILayout.BeginScrollView(editorScroll);
                DrawRowEditor();
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawRowEditor()
        {
            if (selectedRow != null)
            {
                object field;
                FieldInfo[] infos = datatable.GetStructType().GetFields();
                foreach (FieldInfo info in infos)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Height(20f));
                    EditorGUILayout.LabelField(info.Name, GUILayout.MinWidth(200f));
                    GUILayout.Space(5f);
                    field = info.GetValue(selectedRow.row);
                    if (DrawField(info.FieldType, ref field))
                    {
                        info.SetValue(selectedRow.row, field);
                        datatable.SetEntry(selectedRow.key, (T)selectedRow.row);
                        EditorUtility.SetDirty((ScriptableObject)datatable);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void DrawRowHeader()
        {
            if (GUILayout.Button("Add", GUILayout.Width(50f)))
            {
                addAction = true;
            }

            idbox = EditorGUILayout.TextField(idbox, GUILayout.Width(200f));

            if (selectedRow != null && GUILayout.Button("Rename", GUILayout.Width(100f)))
            {
                renameAction = true;
            }

            if (selectedRow != null && GUILayout.Button("Delete", GUILayout.Width(100f)))
            {
                deleteAction = true;
            }
        }

        private void AddAction()
        {
            if (Event.current.type == EventType.Layout && addAction)
            {
                if (string.IsNullOrEmpty(idbox) || datatable.ContainsKey(idbox))
                {
                    int y = 1;
                    idbox = "NewEntry";
                    while (datatable.ContainsKey(idbox))
                    {
                        idbox = "NewEntry_" + y;
                        y++;
                    }
                }

                //todo CTRL - Z
                T r = datatable.CreateStruct();
                DataTableRow uir = new DataTableRow(headers.Count, r, idbox);
                uir.Clicked += OnRowClicked;
                datatable.SetEntry(idbox, r);
                rows.Add(uir);
                EditorUtility.SetDirty((ScriptableObject)datatable);
                GUI.FocusControl("");
                OnSearchUpdated();
                RefreshRowSizes();
                Repaint();
                OnRowClicked(uir, EventArgs.Empty);
            }

            addAction = false;
        }

        private void RenameAction()
        {
            if (Event.current.type == EventType.Layout && renameAction)
            {
                if (string.IsNullOrEmpty(idbox))
                    idbox = selectedRow.key;
                else if (datatable.ContainsKey(idbox))
                {
                    string name = idbox;
                    int y = 1;
                    while (datatable.ContainsKey(name))
                    {
                        name = idbox + "_" + y;
                        y++;
                    }
                    idbox = name;
                }

                if (idbox != selectedRow.key)
                {
                    datatable.DeleteEntry(selectedRow.key);
                    selectedRow.key = idbox;
                    datatable.SetEntry(idbox, (T)selectedRow.row);
                    EditorUtility.SetDirty((ScriptableObject)datatable);
                    GUI.FocusControl("");
                    OnSearchUpdated();
                    Repaint();
                }
            }

            renameAction = false;
        }

        private void DeleteAction()
        {
            if (Event.current.type == EventType.Layout && deleteAction)
            {
                datatable.DeleteEntry(selectedRow.key);
                EditorUtility.SetDirty((ScriptableObject)datatable);
                int index = rows.IndexOf(selectedRow);
                rows.Remove(selectedRow);
                if (rows.Count > index - 1 && rows.Count != 0)
                {
                    OnRowClicked(rows[index - 1], EventArgs.Empty);
                }
                else
                {
                    selectedRow = null;
                    idbox = "";
                    OnSearchUpdated();
                    Repaint();
                }
            }

            deleteAction = false;
        }

        private bool DrawField(Type type, ref object field, params GUILayoutOption[] options)
        {
            bool datapointer = false;
            object newfield = field;
            if (type == typeof(int))
                newfield = EditorGUILayout.IntField((int)field, options);
            else if (type == typeof(float))
                newfield = EditorGUILayout.FloatField((float)field, options);
            else if (type == typeof(double))
                newfield = EditorGUILayout.DoubleField((double)field, options);
            else if (type == typeof(long))
                newfield = EditorGUILayout.LongField((long)field, options);
            else if (type == typeof(Bounds))
                newfield = EditorGUILayout.BoundsField((Bounds)field, options);
            else if (type == typeof(Color))
                newfield = EditorGUILayout.ColorField((Color)field, options);
            else if (type == typeof(AnimationCurve))
                newfield = EditorGUILayout.CurveField((AnimationCurve)field, options);
            else if (type.IsEnum && type.GetCustomAttributes(typeof(FlagsAttribute), false).Any())
                newfield = EditorGUILayout.EnumFlagsField("", (Enum)field, options);
            else if (type.IsEnum)
                newfield = EditorGUILayout.EnumPopup((Enum)field, options);
            else if (type == typeof(Gradient))
                newfield = EditorGUILayout.GradientField((Gradient)field, options);
            else if (type == typeof(LayerMask))
                newfield = EditorGUILayout.LayerField((LayerMask)field, options);
            else if (typeof(DataTablePointer).IsAssignableFrom(type))
            {
                datapointer = DataTablePointerInspector.DrawStandalone((DataTablePointer)field);
            }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                newfield = EditorGUILayout.ObjectField((UnityEngine.Object)field, type, false, options);
            else if (type == typeof(Rect))
                newfield = EditorGUILayout.RectField((Rect)field, options);
            else if (type == typeof(RectInt))
                newfield = EditorGUILayout.RectIntField((RectInt)field, options);
            else if (type == typeof(string))
                newfield = EditorGUILayout.TextField((string)field, options);
            else if (type == typeof(Vector2))
                newfield = EditorGUILayout.Vector2Field("", (Vector2)field, options);
            else if (type == typeof(Vector2Int))
                newfield = EditorGUILayout.Vector2IntField("", (Vector2Int)field, options);
            else if (type == typeof(Vector3))
                newfield = EditorGUILayout.Vector3Field("", (Vector3)field, options);
            else if (type == typeof(Vector3Int))
                newfield = EditorGUILayout.Vector3IntField("", (Vector3Int)field, options);
            else if (type == typeof(Vector4))
                newfield = EditorGUILayout.Vector4Field("", (Vector4)field, options);
            else
                EditorGUILayout.LabelField("[Unable to edit]", options);

            if (newfield != field)
            {
                field = newfield;
                return true;
            }
            if (datapointer)
            {
                return true;
            }
            return false;
        }
    }

}
