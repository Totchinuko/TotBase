using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace TotBase
{
    public class BezierCurve : MonoBehaviour
    {
        [SerializeField]
        protected List<BezierPoint> points = new List<BezierPoint>(2);

        [SerializeField]
        protected int stepPerCurve = 10;

#if UNITY_EDITOR
        public Color bezierColor = Color.white;
#endif

        private const float SPACING = 5f;

        public int PointCount
        {
            get
            {
                return points.Count;
            }
        }

        public int CurveCount
        {
            get
            {
                return points.Count - 1;
            }
        }

        public int StepsCount
        {
            get
            {
                return stepPerCurve * CurveCount;
            }
        }

        public int StepPerCurve
        {
            get => stepPerCurve;
            set
            {
                stepPerCurve = value;
                StepCountChanged?.Invoke(this);
            }
        }

        public event Action<BezierCurve> StepCountChanged;

        public event Action<BezierCurve, BezierPoint> CurveChanged;

        public BezierPoint GetControlPoint(int index)
        {
            return points[index];
        }

        public void Reset()
        {
            points = new List<BezierPoint>();

            points.Add(new BezierPoint(
                new float3(GetSpacing(1f), 0, 0),
                new float3(GetSpacing(2f), 0, 0),
                new float3(GetSpacing(3f), 0, 0)
                ));
            points.Add(new BezierPoint(
                new float3(GetSpacing(4f), 0, 0),
                new float3(GetSpacing(5f), 0, 0),
                new float3(GetSpacing(6f), 0, 0)
                ));
            StepCountChanged?.Invoke(this);
        }

        private void Awake()
        {
            foreach (BezierPoint point in points)
                point.PointChanged += OnPointChanged;
        }

        private void OnPointChanged(BezierPoint obj)
        {
            CurveChanged?.Invoke(this, obj);
        }

        public float3 GetPoint(float t)
        {
            return transform.TransformPoint(GetLocalPoint(t));
        }

        public float3 GetPointAtStep(int index)
        {
            return GetPoint(index / (float)StepsCount);
        }

        public float3 GetLocalPoint(float t)
        {
            if (t >= 1)
                return points[PointCount - 1].Point;

            float localT;
            int i = GetPointIndex(t, out localT);
            return GetLocalPointOnCurve(points[i], points[i + 1], localT);
        }

        public float3 GetLocalPointOnCurve(BezierPoint start, BezierPoint end, float t)
        {
            return BezierUtils.GetPoint(start.Point, start.Post, end.Previous, end.Point, t);
        }

        public float3 GetDirectionAtStep(int index)
        {
            return GetDirection(index / (float)StepsCount);
        }

        public float3 GetDirection(float t)
        {
            if (t >= 1)
                return GetDirectionOnCurve(points[PointCount - 2], points[PointCount - 1], 1f);

            float localT;
            int i = GetPointIndex(t, out localT);
            return GetDirectionOnCurve(points[i], points[i + 1], localT);
        }

        public float GetWeight(float t)
        {
            if (t >= 1)
                return points[PointCount - 1].Weight;

            float localT;
            int i = GetPointIndex(t, out localT);
            return math.lerp(points[i].Weight, points[i + 1].Weight, localT);
        }

        public float GetWeightAtStep(int index)
        {
            return GetWeight(index / (float)StepsCount);
        }

        public int GetPointIndex(float t, out float localT)
        {
            localT = math.clamp(t, 0, 1);
            localT *= CurveCount;
            int i = (int)math.floor(localT);
            localT -= i;

            return i;
        }

        public float3 GetDirectionOnCurve(BezierPoint start, BezierPoint end, float t)
        {
            return transform.TransformPoint(BezierUtils.GetFirstDerivative(
                start.Point, start.Post, end.Previous, end.Point, t)) - transform.position;
        }

        public void InsertCurve(int index)
        {
            float3 direction, point;
            BezierPoint newPoint;

            if (index == CurveCount)
            {
                point = points[CurveCount].Post;
                direction = math.normalize(points[CurveCount].Post - points[CurveCount].Point);
                newPoint = new BezierPoint(point + GetSpacing(1f) * direction, point + GetSpacing(2f) * direction, point + GetSpacing(3f) * direction);

                points.Add(newPoint);
            }
            else
            {
                BezierPoint start = points[index];
                BezierPoint end = points[index + 1];

                point = GetLocalPointOnCurve(start, end, .5f);
                direction = math.normalize(GetDirectionOnCurve(start, end, .5f));

                newPoint = new BezierPoint(point - direction * GetSpacing(1f), point, point + direction * GetSpacing(1f));

                points.Insert(index + 1, newPoint);
            }
            newPoint.PointChanged += OnPointChanged;
            StepCountChanged?.Invoke(this);
        }

        public void RemoveCurve(int index)
        {
            if (points.Count > 2 && points.Count > index)
            {
                points[index].PointChanged -= OnPointChanged;
                points.RemoveAt(index);
                StepCountChanged?.Invoke(this);
            }
        }

        private float GetSpacing(float idx)
        {
            return SPACING * idx;
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Color bu = Gizmos.color;
            Gizmos.color = bezierColor;
            Vector3 start = GetPointAtStep(0);
            for (int i = 1; i <= StepsCount; i++)
            {
                Vector3 end = GetPointAtStep(i);
                Gizmos.DrawLine(start, end);
                start = end;
            }
            Gizmos.color = bu;
        }

#endif
    }
}