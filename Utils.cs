using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace TotBase
{
    public static class Utils
    {
        public static Vector2 MousePosToScreenUV(Vector2 mousePosition, bool normalized = true)
        {
            mousePosition.x /= Screen.width;
            mousePosition.y /= Screen.height;
            if (normalized)
            {
                float ratio = Screen.width / Screen.height;
                if (ratio < 1f)
                    mousePosition.y *= ratio;
                else
                    mousePosition.x /= ratio;
            }
            return mousePosition;
        }

        /// <summary>
        /// Use 4 raycast for smooth normal calculation
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static void GetFourPointGroundData(Vector3 origin, Vector3 forward, float radius, float distance, float maxGroundAngle, LayerMask mask, ref GroundData data, bool debug = false)
        {
            // testing if we are on the ground first
            RaycastHit hit;
            if (!Physics.SphereCast(origin, radius, -Vector3.up, out hit, distance, mask, QueryTriggerInteraction.Ignore))
            {
                data.onGround = false;
                data.position = hit.point;
                return;
            }

            Vector3 right = Vector3.Cross(Vector3.up, forward);
            float addedDistance = GetCheckDistance(radius, maxGroundAngle);

            Ray rayA = new Ray(origin + (forward + right).normalized * radius, Vector3.down);
            Ray rayB = new Ray(origin + (forward + -right).normalized * radius, Vector3.down);
            Ray rayC = new Ray(origin + (-forward + right).normalized * radius, Vector3.down);
            Ray rayD = new Ray(origin + (-forward + -right).normalized * radius, Vector3.down);

            bool hittingA = Physics.SphereCast(rayA, 0.02f, out RaycastHit hitA, distance + addedDistance, mask, QueryTriggerInteraction.Ignore);
            bool hittingB = Physics.SphereCast(rayB, 0.02f, out RaycastHit hitB, distance + addedDistance, mask, QueryTriggerInteraction.Ignore);
            bool hittingC = Physics.SphereCast(rayC, 0.02f, out RaycastHit hitC, distance + addedDistance, mask, QueryTriggerInteraction.Ignore);
            bool hittingD = Physics.SphereCast(rayD, 0.02f, out RaycastHit hitD, distance + addedDistance, mask, QueryTriggerInteraction.Ignore);

            // more complexe ground checking
            if ((!hittingA && !hittingD) || (!hittingC && !hittingB))
            {
                data.onGround = false;
                data.position = hit.point;
                return;
            }

            Vector3 pointA = hittingA ? hitA.point : hitD.point + (hit.point - hitD.point) * 2;
            Vector3 pointB = hittingB ? hitB.point : hitC.point + (hit.point - hitC.point) * 2;
            Vector3 pointC = hittingC ? hitC.point : hitB.point + (hit.point - hitB.point) * 2;
            Vector3 pointD = hittingD ? hitD.point : hitA.point + (hit.point - hitA.point) * 2;

            data.onGround = true;
            data.position = hit.point;
            data.normal = Vector3.Cross(pointA - pointD, pointC - pointB).normalized;
            data.angle = Mathf.Round(Vector3.Angle(data.normal, Vector3.up));

            if (debug)
            {
                DebugDraw.DrawMarker(pointA, .1f, hittingA ? Color.yellow : Color.red, 0f, false);
                DebugDraw.DrawMarker(pointB, .1f, hittingB ? Color.yellow : Color.red, 0f, false);
                DebugDraw.DrawMarker(pointC, .1f, hittingC ? Color.yellow : Color.red, 0f, false);
                DebugDraw.DrawMarker(pointD, .1f, hittingD ? Color.yellow : Color.red, 0f, false);
            }
        }

        /// <summary>
        /// Get the ground distance check depending on radius check and max angle actor can stand on
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="maxDegAngle"></param>
        /// <returns></returns>
        public static float GetCheckDistance(float radius, float maxDegAngle)
        {
            return Mathf.Tan(maxDegAngle * Mathf.Deg2Rad) * radius;
        }

        public static Vector3 GetRandomPointInCircle(Vector3 center, Vector3 direction, float radius)
        {
            // Find the center of the random circle in front of the camera
            center = center + direction.normalized * radius;
            Vector2 center2D = new Vector2(center.x, center.z);
            Vector2 rand = center2D + UnityEngine.Random.insideUnitCircle * radius;
            return new Vector3(rand.x, center.y, rand.y);
        }

        public static Vector3 GetRandomDirectionPlane(Vector3 direction, Vector3 normal, float maxAngle)
        {
            return Quaternion.AngleAxis(UnityEngine.Random.Range(-(maxAngle / 2), maxAngle / 2), normal) * direction;
        }

        public static Vector3 GetRandomDirection(Vector3 direction, float maxAngle)
        {
            Vector3 randDirection = UnityEngine.Random.insideUnitSphere;
            float angle = Vector3.Angle(direction, randDirection);
            if (angle > maxAngle)
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
        public static T ConfigureDelegate<T, R>(R machine, T methodRoot, Dictionary<Enum, Dictionary<int, Delegate>> cache) where T : class where R : class, IStateMachine
        {
            Delegate root = methodRoot as Delegate;
            // create cache if it don't exist already for the given state
            Dictionary<int, Delegate> lookup;
            if (!cache.TryGetValue(machine.CurrentState, out lookup))
                cache[machine.CurrentState] = lookup = new Dictionary<int, Delegate>();

            Delegate returnValue;
            int hash = root.Method.Name.GetHashCode();
            if (!lookup.TryGetValue(hash, out returnValue))
            {
                // find using reflexion a method in the current type that match a naming convention (State_RootMethode())
                MethodInfo method = machine.GetType().GetMethod($"{machine.CurrentState.ToString()}_{root.Method.Name}", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

                // use passed default if no matching method exists and store the result in cache
                if (method != null)
                    returnValue = Delegate.CreateDelegate(typeof(T), machine, method);
                lookup[hash] = returnValue;
            }
            return returnValue as T;
        }

        public static string GetDocumentPathOrCreate()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = Path.Combine(path, Application.companyName, Application.productName);
            Directory.CreateDirectory(path);
            return path;
        }

        public static bool TryGetFile(out string absolutePath, params string[] relativePath) => TryGetFile(out absolutePath, Path.Combine(relativePath));

        public static bool TryGetFile(out string absolutePath, string relativePath)
        {
            absolutePath = Path.Combine(GetDocumentPathOrCreate(), relativePath);
            if (File.Exists(absolutePath))
                return true;
            return false;
        }

        public static bool TryGetFileContent(out string content, params string[] relativePath) => TryGetFileContent(out content, Path.Combine(relativePath));

        public static bool TryGetFileContent(out string content, string relativePath)
        {
            content = "";
            if (TryGetFile(out string path, relativePath))
            {
                content = File.ReadAllText(path);
                return true;
            }
            return false;
        }

        public static bool TrySetFileContent(string content, params string[] relativePath) => TrySetFileContent(content, Path.Combine(relativePath));

        public static bool TrySetFileContent(string content, string relativePath)
        {
            try
            {
                string path = Path.Combine(GetDocumentPathOrCreate(), relativePath);
                File.WriteAllText(path, content);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static void WaitNextFrame(Action callback, Segment segment = Segment.Update)
        {
            Timing.RunCoroutine(WaitNextFrameInternal(callback));
        }

        private static IEnumerator<float> WaitNextFrameInternal(Action callback)
        {
            yield return Timing.WaitForOneFrame;
            callback?.Invoke();
        }

        public static Vector3 GetStart(this CapsuleCollider capsule)
        {
            return capsule.transform.TransformPoint(capsule.center + Vector3.up * capsule.height / 2);
        }

        public static Vector3 GetEnd(this CapsuleCollider capsule)
        {
            return capsule.transform.TransformPoint(capsule.center - Vector3.up * capsule.height / 2);
        }

        public static Vector3 GetCenter(this CapsuleCollider capsule)
        {
            return capsule.transform.TransformPoint(capsule.center);
        }

        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }

        public static object GetDefault(object component, string path)
        {
            return GetDefault(GetTypeAtPath(component, path));
        }

        public static object GetValueAtPath(object component, string path)
        {
            foreach (string segment in path.Split('.'))
            {
                if (component == null) return null;
                component = GetValue(component, segment);
            }
            return component;
        }

        public static void SetValueAtPath(object component, string path, object value)
        {
            string[] segments = path.Split('.');
            for (int i = 0; i < segments.Length; i++)
            {
                if (component == null) return;
                if (i == segments.Length - 1)
                    SetValue(component, segments[i], value);
                else
                    component = GetValue(component, segments[i]);
            }
        }

        public static Type GetTypeAtPath(object component, string path)
        {
            PropertyDescriptor property = null;
            foreach (string segment in path.Split('.'))
            {
                if (component == null) return null;
                property = TypeDescriptor.GetProperties(component)?[segment];
                component = property?.GetValue(component);
            }
            return property?.PropertyType;
        }

        public static object GetValue(object component, string propertyName)
        {
            return TypeDescriptor.GetProperties(component)?[propertyName]?.GetValue(component);
        }

        public static void SetValue(object component, string propertyName, object value)
        {
            TypeDescriptor.GetProperties(component)?[propertyName]?.SetValue(component, value);
        }

        public static bool TestPlanesAABB(NativeArray<Plane> planes, Bounds bounds)
        {
            for (int i = 0; i < planes.Length; i++)
            {
                Plane plane = planes[i];
                float3 normal_sign = math.sign(plane.normal);
                float3 test_point = (float3)(bounds.center) + (bounds.extents * normal_sign);

                float dot = math.dot(test_point, plane.normal);
                if (dot + plane.distance < 0)
                    return false;
            }

            return true;
        }
    }
}