using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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

        public static Vector3 FromVector(this Vector3 vec1, Vector3 vec2)
        {
            vec1.x = vec2.x;
            vec1.y = vec2.y;
            vec1.z = vec2.z;

            return vec1;
        }

        public static Vector3 FromVector(this Vector3 vec1, Vector2 vec2, float z)
        {
            vec1.x = vec2.x;
            vec1.y = vec2.y;
            vec1.z = z;

            return vec1;
        }

        public static Vector2 FromVector(this Vector2 vec1, Vector2 vec2)
        {
            vec1.x = vec2.x;
            vec1.y = vec2.y;

            return vec1;
        }

        public static Vector2 FromVector(this Vector2 vec1, Vector3 vec2)
        {
            vec1.x = vec2.x;
            vec1.y = vec2.y;

            return vec1;
        }

        public static Vector2 Snap(this Vector2 pos, float gridSize)
        {
            pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
            pos.y = Mathf.Round(pos.y / gridSize) * gridSize;
            return pos;
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

        public static Vector3 GetRandomPointInCircle(Vector3 center, Vector3 direction, float radius)
        {
            // Find the center of the random circle in front of the camera
            center = center + direction.normalized * radius;
            Vector2 center2D = new Vector2(center.x, center.z);
            Vector2 rand = center2D + UnityEngine.Random.insideUnitCircle * radius;
            return new Vector3(rand.x, center.y, rand.y);
        }

        public static Vector3 GetRandomDirection(Vector3 direction, float maxAngle)
        {
            Vector3 randDirection = UnityEngine.Random.insideUnitSphere;
            float angle = Vector3.Angle(direction, randDirection);
            if(angle > maxAngle)
            {
                float lerp = UnityEngine.Random.Range(0f, maxAngle / angle);
                return Vector3.Lerp(direction, randDirection, lerp);
            }
                
            return randDirection;

        }

        public static IEnumerable<T> GetObjectWithInterface<T>() where T : class
        {
            // use reflection to get all QualityDefinition and populate the QualityLevel
            return from t in GetClassWithInterface<T>()
                   where t.GetConstructor(Type.EmptyTypes) != null
                   select Activator.CreateInstance(t) as T;
        }

        public static IEnumerable<Type> GetClassWithInterface<T>() where T : class
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(T).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract);
        }

        public static IEnumerable<Type> GetClassWithAttribute<T>() where T : Attribute
        {
            return from t in Assembly.GetExecutingAssembly().GetTypes()
                   where t.GetAttribute<T>() != null
                   select t;
        }

        public static bool TryParseVector2Int(string serialized, out Vector2Int value)
        {
            value = Vector2Int.zero;
            int x, y;
            string[] split = serialized.Split(';');
            if (split.Length == 2 && int.TryParse(split[0], out x) && int.TryParse(split[1], out y))
            {
                value = new Vector2Int(x, y);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Use reflexion to find matching method given the current state and main method name. Build a cache for futur lookup. Use default if none found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodRoot"></param>
        /// <param name="defaultMethod"></param>
        /// <returns></returns>
        public static T ConfigureDelegate<T, R>(R machine, string methodRoot, T defaultMethod, Dictionary<Enum, Dictionary<string, Delegate>> cache) where T : class where R : class, IStateMachine
        {
            // create cache if it don't exist already for the given state
            Dictionary<string, Delegate> lookup;
            if (!cache.TryGetValue(machine.CurrentState, out lookup))
                cache[machine.CurrentState] = lookup = new Dictionary<string, Delegate>();

            Delegate returnValue;
            if (!lookup.TryGetValue(methodRoot, out returnValue))
            {
                // find using reflexion a method in the current type that match a naming convention (State_RootMethode())
                MethodInfo method = machine.GetType().GetMethod(machine.CurrentState.ToString() + "_" + methodRoot, BindingFlags.Instance
                    | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

                // use passed default if no matching method exists and store the result in cache
                if (method != null)
                    returnValue = Delegate.CreateDelegate(typeof(T), machine, method);
                else
                    returnValue = defaultMethod as Delegate;

                lookup[methodRoot] = returnValue;
            }
            return returnValue as T;
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

        /// <summary>
        /// Returns all monobehaviours (casted to T)
        /// </summary>
        /// <typeparam name="T">interface type</typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        public static T[] GetInterfaces<T>(this GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
            var mObjs = gObj.GetComponents<MonoBehaviour>();

            return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
        }

        /// <summary>
        /// Returns the first monobehaviour that is of the interface type (casted to T)
        /// </summary>
        /// <typeparam name="T">Interface type</typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        public static T GetInterface<T>(this GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
            return gObj.GetInterfaces<T>().FirstOrDefault();
        }

        /// <summary>
        /// Returns the first instance of the monobehaviour that is of the interface type T (casted to T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        public static T GetInterfaceInChildren<T>(this GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
            return gObj.GetInterfacesInChildren<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets all monobehaviours in children that implement the interface of type T (casted to T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        public static T[] GetInterfacesInChildren<T>(this GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");

            var mObjs = gObj.GetComponentsInChildren<MonoBehaviour>();

            return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
        }
    }
}
