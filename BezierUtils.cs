using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace TotBase
{
    public static class BezierUtils
    {
        // quadratic bezier methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetPoint(float3 p0, float3 p1, float3 p2, float t)
        {
            t = math.clamp(t, 0f, 1f);
            return (1 - t) * (1 - t) * p0 + 2f * (1 - t) * t * p1 + t * t * p2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetFirstDerivative(float3 p0, float3 p1, float3 p2, float t)
        {
            return
                2f * (1f - t) * (p1 - p0) +
                2f * t * (p2 - p1);
        }

        // bicubic bezier methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetPoint(float3 p0, float3 p1, float3 p2, float3 p3, float t)
        {
            t = math.clamp(t, 0, 1);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * oneMinusT * p0 +
                3f * oneMinusT * oneMinusT * t * p1 +
                3f * oneMinusT * t * t * p2 +
                t * t * t * p3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 GetFirstDerivative(float3 p0, float3 p1, float3 p2, float3 p3, float t)
        {
            t = math.clamp(t, 0, 1);
            float oneMinusT = 1f - t;
            return
                3f * oneMinusT * oneMinusT * (p1 - p0) +
                6f * oneMinusT * t * (p2 - p1) +
                3f * t * t * (p3 - p2);
        }
    }
}