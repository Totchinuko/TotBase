using System;
using Unity.Mathematics;
using UnityEngine;

namespace TotBase
{
    [Serializable]
    public class BezierPoint
    {
        [SerializeField]
        private float3 previous;

        [SerializeField]
        private float3 point;

        [SerializeField]
        private float3 post;

        [SerializeField]
        private float weight = 1f;

        [SerializeField]
        private BezierControlPointMode mode;

        public event Action<BezierPoint> PointChanged;

        public float3 Point
        {
            get { return point; }
            set
            {
                float3 delta = value - point;
                previous += delta;
                post += delta;
                point = value;
                PointChanged?.Invoke(this);
            }
        }

        public float3 Previous
        {
            get { return previous; }
            set
            {
                previous = value;
                post = EnforceMode(post, previous);
                PointChanged?.Invoke(this);
            }
        }

        public float Weight
        {
            get => weight;
            set
            {
                weight = value;
                PointChanged?.Invoke(this);
            }
        }

        public float3 Post
        {
            get { return post; }
            set
            {
                post = value;
                previous = EnforceMode(previous, post);
                PointChanged?.Invoke(this);
            }
        }

        public BezierControlPointMode Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                post = EnforceMode(post, previous);
                PointChanged?.Invoke(this);
            }
        }

        public BezierPoint(float3 previous, float3 point, float3 post, BezierControlPointMode mode = BezierControlPointMode.Free)
        {
            this.previous = previous;
            this.point = point;
            this.post = post;
            this.mode = mode;
        }

        private float3 EnforceMode(float3 enforcedPoint, float3 fixedPoint)
        {
            if (mode == BezierControlPointMode.Free)
                return enforcedPoint;

            float3 enforcedTangent = point - fixedPoint;

            if (mode == BezierControlPointMode.Aligned)
                enforcedTangent = math.normalize(enforcedTangent) * math.distance(point, enforcedPoint);

            enforcedPoint = point + enforcedTangent;

            return enforcedPoint;
        }
    }
}