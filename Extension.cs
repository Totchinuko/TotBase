using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;

namespace TotBase
{
    public static class Extension
    {
        public static string ToConsole(this string str)
        {
            return str.Replace(" ", "_").ToLower();
        }

        public static float Remap(this float value, float from1, float from2, float to1, float to2)
        {
            return to1 + (value - from1) * (to2 - to1) / (from2 - from1);
        }

        public static float Remap(this float value, float from1, float from2, DoubleFloat to) => Remap(value, from1, from2, to.min, to.max);

        public static float Remap(this int value, float from1, float from2, float to1, float to2)
        {
            return to1 + (value - from1) * (to2 - to1) / (from2 - from1);
        }

        public static float RemapC(this float value, float from1, float from2, float to1, float to2)
        {
            if (to1 < to2)
                return Mathf.Clamp(value.Remap(from1, from2, to1, to2), to1, to2);
            else if (to1 > to2)
                return Mathf.Clamp(value.Remap(from1, from2, to1, to2), to2, to1);
            else
                return to1;
        }

        public static float RemapC(this int value, float from1, float from2, float to1, float to2)
        {
            if (to1 < to2)
                return Mathf.Clamp(value.Remap(from1, from2, to1, to2), to1, to2);
            else if (to1 > to2)
                return Mathf.Clamp(value.Remap(from1, from2, to1, to2), to2, to1);
            else
                return to1;
        }

        public static bool VEqual(this Vector2Int a, Vector2Int b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static Transform FirstChildOrDefault(this Transform parent, Func<Transform, bool> query)
        {
            if (parent.childCount == 0)
            {
                return null;
            }

            Transform result = null;
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (query(child))
                {
                    return child;
                }
                result = FirstChildOrDefault(child, query);
                if(result != null)
                    break;
            }

            return result;
        }

        public static Color ApplyHSBEffect(this Color value, Vector4 hsb)
        {
            float hue = 360 * hsb.x;
            float brightness = hsb.y * 2 - 1;
            float contrast = hsb.z * 2;
            float saturation = hsb.w * 2;

            Vector3 rgb = new Vector3(value.r, value.g, value.b);
            rgb = rgb.ApplyHue(hue);
            rgb = new Vector3(rgb.x - 0.5f, rgb.y - 0.5f, rgb.z - 0.5f) * contrast;
            rgb = new Vector3(rgb.x + 0.5f, rgb.y + 0.5f, rgb.z + 0.5f);
            rgb = new Vector3(rgb.x + brightness, rgb.y + brightness, rgb.z + brightness);
            float intensity = Vector3.Dot(rgb, new Vector3(0.299f, 0.587f, 0.114f));
            rgb = Vector3.Lerp(new Vector3(intensity, intensity, intensity), rgb, saturation);

            return new Color(rgb.x, rgb.y, rgb.z, value.a);
        }

        public static Vector3 ApplyHue(this Vector3 color, float hue)
        {
            float angle = Mathf.Rad2Deg * hue;
            Vector3 k = new Vector3(0.57735f, 0.57735f, 0.57735f);
            float cosAngle = Mathf.Cos(angle);
            // Rodrigues roation formula
            return color * cosAngle + Vector3.Cross(k, color) * Mathf.Sin(angle) + k * Vector3.Dot(k, color) * (1 - cosAngle);
        }

        private static System.Random rng = new System.Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }

        // Quick extension method to get the first attribute of type T on a given Type
        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            if (type == null)
                throw new ArgumentNullException("type");
            var attributes = type.GetCustomAttributes(typeof(T), false);
            if (attributes != null && attributes.Length > 0)
                return (T)attributes[0];
            else
                return null;
        }

        public static Quaternion ClampToIdentity(this Quaternion q, float angle)
        {
            return q.ClampFrom(Quaternion.identity, angle);
        }

        public static Quaternion ClampFrom(this Quaternion q, Quaternion reference, float angle)
        {
            float currentAngle = Quaternion.Angle(q, reference);
            if (currentAngle > angle)
                return Quaternion.Slerp(reference, q, angle / currentAngle);
            return q;
        }
        public static bool Contains<T>(this T[] t_arr, T t) where T : class
        {
            for (int i = 0; i < t_arr.Length; i++)
                if (t_arr[i] == t)
                    return true;
            return false;
        }

        public static bool EqualTreshold(this Vector3 v, Vector3 u, float treshold)
        {
            return Mathf.Abs(v.x - u.x) < treshold && Mathf.Abs(v.y - u.y) < treshold && Mathf.Abs(v.z - u.z) < treshold;
        }

        public static bool EqualTreshold(this Vector2 v, Vector2 u, float treshold)
        {
            return Mathf.Abs(v.x - u.x) < treshold && Mathf.Abs(v.y - u.y) < treshold;
        }

        public static bool EqualTreshold(this float v, float u, float treshold)
        {
            return Mathf.Abs(v - u) < treshold;
        }

        public static Vector2 Snap(this Vector2 pos, float gridSize)
        {
            pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
            pos.y = Mathf.Round(pos.y / gridSize) * gridSize;
            return pos;
        }

        public static Vector2Int Min(this Vector2Int v1, Vector2Int v2) {
            return new Vector2Int(Math.Min(v1.x, v2.x), Math.Min(v1.y, v2.y));
        }

        public static Vector2Int Max(this Vector2Int v1, Vector2Int v2) {
            return new Vector2Int(Math.Max(v1.x, v2.x), Math.Max(v1.y, v2.y));
        }

        public static Rect SnapToGrid(this Rect rect, float gridSize)
        {            
            rect.position = rect.position.Snap(gridSize);
            return rect;
        }

        public static Rect SnapCenterToGrid(this Rect rect, float gridSize)
        {
            rect.center = rect.center.Snap(gridSize);
            return rect;
        }


        public static Rect Extends(this Rect rect, float amount)
        {
            rect.position -= new Vector2(amount, amount);
            rect.size += new Vector2(amount * 2, amount * 2);
            return rect;
        }


        public static void DrawTexture(this Texture2D tex, Rect rect, Color col)
        {
            Color old = GUI.color;
            GUI.color = col;
            GUI.DrawTexture(rect, tex);
            GUI.color = old;
        }

        public static Rect Include(this Rect rect, Rect r)
        {
            rect.xMax = Mathf.Max(r.xMax, rect.xMax);
            rect.xMin = Mathf.Min(r.xMin, rect.xMin);
            rect.yMax = Mathf.Max(r.yMax, rect.yMax);
            rect.yMin = Mathf.Min(r.yMin, rect.yMin);
            return rect;
        }

        public static bool IsIncluding(this Rect rect, Rect r)
        {
            return rect.xMax >= r.xMax && rect.yMax >= r.yMax && rect.xMin <= r.xMin && rect.yMin <= r.yMin;
        }

        public static Vector3 ToPolarCoordinate(this Vector3 cartesian)
        {
            float mag = cartesian.magnitude;
            return new Vector3()
            {
                x = mag,
                y = Mathf.Acos(cartesian.y / mag),
                z = Mathf.Atan(cartesian.x / cartesian.z)
            };
        }

        public static Vector3 ToCartesianCoordinate(this Vector3 polar)
        {
            return new Vector3()
            {
                z = polar.x * Mathf.Sin(polar.y) * Mathf.Cos(polar.z),
                x = polar.x * Mathf.Sin(polar.y) * Mathf.Sin(polar.z),
                y = polar.x * Mathf.Cos(polar.y)
            };
        }

        public static Bounds Constraint(this Bounds bounds, Bounds constraint)
        {
            Vector3 max = bounds.max;
            Vector3 min = bounds.min;

            max.x = max.x > constraint.max.x ? constraint.max.x : max.x;
            max.y = max.y > constraint.max.y ? constraint.max.y : max.y;
            max.z = max.z > constraint.max.z ? constraint.max.z : max.z;

            min.x = min.x < constraint.min.x ? constraint.min.x : min.x;
            min.y = min.y < constraint.min.y ? constraint.min.y : min.y;
            min.z = min.z < constraint.min.z ? constraint.min.z : min.z;

            bounds.SetMinMax(min, max);
            return bounds;
        }
    }
}
