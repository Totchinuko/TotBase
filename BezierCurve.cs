using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace TotBase
{
    public class BezierCurve : MonoBehaviour
    {
        [SerializeField]
        protected List<BezierPoint> points = new List<BezierPoint>(2);

        public int stepPerCuve = 10;

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
        }

        public float3 GetPoint(float t)
        {
            return transform.TransformPoint(GetLocalPoint(t));
        }

        public float3 GetLocalPoint(float t)
        {
            if (t >= 1)
                return points[CurveCount].Point;

            float localT;
            int i = GetPointIndex(t, out localT);
            return GetLocalPointOnCurve(points[i], points[i + 1], localT);
        }

        public float3 GetLocalPointOnCurve(BezierPoint start, BezierPoint end, float t)
        {
            return BezierUtils.GetPoint(start.Point, start.Post, end.Previous, end.Point, t);
        }

        public float3 GetVelocity(float t)
        {
            if (t >= 1)
                return points[CurveCount].Point;

            float localT;
            int i = GetPointIndex(t, out localT);
            return GetVelocityOnCurve(points[i], points[i + 1], localT);
        }

        public int GetPointIndex(float t, out float localT)
        {
            localT = math.clamp(t, 0, 1);
            localT *= CurveCount;
            int i = (int)math.floor(localT);
            localT -= i;

            return i;
        }

        public float3 GetVelocityOnCurve(BezierPoint start, BezierPoint end, float t)
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
                direction = math.normalize(GetVelocityOnCurve(start, end, .5f));

                newPoint = new BezierPoint(point - direction * GetSpacing(1f), point, point + direction * GetSpacing(1f));

                points.Insert(index + 1, newPoint);
            }
        }

        public void RemoveCurve(int index)
        {
            if (points.Count > 2 && points.Count > index)
                points.RemoveAt(index);
        }

        private float GetSpacing(float idx)
        {
            return SPACING * idx;
        }
    }
}