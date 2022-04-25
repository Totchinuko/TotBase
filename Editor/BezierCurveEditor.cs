using UnityEditor;
using UnityEngine;

namespace TotBase
{
    [CustomEditor(typeof(BezierCurve))]
    public class BezierCurveEditor : Editor
    {
        public BezierCurve curve;
        public Transform tr;
        public Quaternion handleRotation;

        private const float handleSize = 0.04f;
        private const float pickSize = 0.06f;

        public int selectedCurve = -1;

        private static readonly Color[] modeColors = {
            Color.white,
            Color.yellow,
            Color.cyan
        };

        private void OnSceneGUI()
        {
            curve = target as BezierCurve;
            tr = curve.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ? tr.rotation : Quaternion.identity;

            // show handles
            for (int i = 0; i < curve.PointCount; i++)
                DrawBezierPoint(i);

            //DrawBezier(curve, Color.white);
        }

        public override void OnInspectorGUI()
        {
            curve = target as BezierCurve;

            if (selectedCurve >= 0 && selectedCurve < curve.PointCount)
            {
                DrawSelectectPointInspector();

                EditorGUI.BeginChangeCheck();
                BezierPoint bezierPoint = curve.GetControlPoint(selectedCurve);
                BezierControlPointMode mode = (BezierControlPointMode)
                    EditorGUILayout.EnumPopup("Mode", bezierPoint.Mode);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(curve, "Change Point Mode");
                    bezierPoint.Mode = mode;
                    EditorUtility.SetDirty(curve);
                }
            }

            GUILayout.Space(15);

            EditorGUI.BeginChangeCheck();
            int stepPerCurve = EditorGUILayout.IntField("Steps per curve", curve.StepPerCurve);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Change Step per Curve");
                curve.StepPerCurve = stepPerCurve;
                EditorUtility.SetDirty(curve);
            }

            EditorGUI.BeginChangeCheck();
            Color bColor = EditorGUILayout.ColorField("Curve Color", curve.bezierColor);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Change Bezier color");
                curve.bezierColor = bColor;
                EditorUtility.SetDirty(curve);
            }

            GUILayout.Space(15);

            if (selectedCurve >= 0 && selectedCurve < curve.PointCount)
            {
                if (GUILayout.Button("Add Segment"))
                {
                    Undo.RecordObject(curve, "Add Curve");
                    curve.InsertCurve(selectedCurve);
                    selectedCurve += 1;
                    EditorUtility.SetDirty(curve);
                }

                if (GUILayout.Button("Remove Segment"))
                {
                    Undo.RecordObject(curve, "Remove Curve");
                    curve.RemoveCurve(selectedCurve);
                    selectedCurve = -1;
                    EditorUtility.SetDirty(curve);
                }
            }
        }

        private void DrawBezierPoint(int index)
        {
            BezierPoint bezier = this.curve.GetControlPoint(index);

            Handles.color = modeColors[(int)bezier.Mode];

            // previous handle
            Vector3 previous = tr.TransformPoint(bezier.Previous);
            float previousSize = HandleUtility.GetHandleSize(previous);
            Vector3 point = tr.TransformPoint(bezier.Point);
            float pointSize = HandleUtility.GetHandleSize(point);
            Vector3 post = tr.TransformPoint(bezier.Post);
            float postSize = HandleUtility.GetHandleSize(post);

            if (Handles.Button(previous, handleRotation, handleSize * previousSize, pickSize * previousSize, Handles.DotHandleCap) ||
                Handles.Button(point, handleRotation, handleSize * pointSize, pickSize * pointSize, Handles.DotHandleCap) ||
                Handles.Button(post, handleRotation, handleSize * postSize, pickSize * postSize, Handles.DotHandleCap))
            {
                selectedCurve = index;
                Repaint();
            }

            Handles.color = Color.white;
            if (selectedCurve == index)
            {
                EditorGUI.BeginChangeCheck();
                previous = Handles.DoPositionHandle(previous, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(curve, "Move Point");
                    EditorUtility.SetDirty(curve);
                    bezier.Previous = tr.InverseTransformPoint(previous);
                }

                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(curve, "Move Point");
                    EditorUtility.SetDirty(curve);
                    bezier.Point = tr.InverseTransformPoint(point);
                }

                EditorGUI.BeginChangeCheck();
                post = Handles.DoPositionHandle(post, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(curve, "Move Point");
                    EditorUtility.SetDirty(curve);
                    bezier.Post = tr.InverseTransformPoint(post);
                }

                Handles.color = Color.green;
            }

            Handles.DrawLine(tr.TransformPoint(bezier.Previous), tr.TransformPoint(bezier.Point));
            Handles.DrawLine(tr.TransformPoint(bezier.Post), tr.TransformPoint(bezier.Point));
        }

        //public void DrawBezier(BezierCurve bezier, Color color)
        //{
        //    Color c = Handles.color;
        //    Handles.color = color;

        //    Vector3 start = bezier.GetPoint(0f);
        //    int steps = bezier.stepPerCuve * bezier.CurveCount;
        //    for (int i = 1; i <= steps; i++)
        //    {
        //        Vector3 end = bezier.GetPoint(i / (float)steps);
        //        Handles.DrawLine(start, end);
        //        start = end;
        //    }

        //    Handles.color = c;
        //}

        public void DrawSelectectPointInspector()
        {
            BezierPoint bezierPoint = curve.GetControlPoint(selectedCurve);

            EditorGUI.BeginChangeCheck();
            Vector3 previous = EditorGUILayout.Vector3Field("Previous", bezierPoint.Previous);
            Vector3 point = EditorGUILayout.Vector3Field("Point", bezierPoint.Point);
            Vector3 post = EditorGUILayout.Vector3Field("Post", bezierPoint.Post);
            float weight = EditorGUILayout.FloatField("Weight", bezierPoint.Weight);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Move Point");
                bezierPoint.Previous = previous;
                bezierPoint.Point = point;
                bezierPoint.Post = post;
                bezierPoint.Weight = weight;
                EditorUtility.SetDirty(curve);
            }
        }
    }
}