using UnityEngine;
using System;

namespace TotBase
{
    public static class TotPhysics
    {
        
        /// <summary>
        /// Use 4 raycast for smooth normal calculation
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static void GetFourPointGroundData(Vector3 origin, Vector3 forward, float radius, float distance, float maxGroundAngle, LayerMask mask, ref GroundData data, bool debug = false)
        {
            // testing if we are on the ground first            
            RaycastHit hit;
            if(!Physics.SphereCast(origin, radius, -Vector3.up, out hit, distance, mask, QueryTriggerInteraction.Ignore))
            {
                data.onGround = false;
                data.position = hit.point;
                return;
            }

            Vector3 right = Vector3.Cross(Vector3.up, forward);
            float addedDistance = TotPhysics.GetCheckDistance(radius, maxGroundAngle);
            
            Ray rayA = new Ray(origin + (forward + right).normalized * radius, Vector3.down);
            Ray rayB = new Ray(origin + (forward + -right).normalized * radius, Vector3.down);
            Ray rayC = new Ray(origin + (-forward + right).normalized * radius, Vector3.down);
            Ray rayD = new Ray(origin + (-forward + -right).normalized * radius, Vector3.down);

            bool hittingA = Physics.SphereCast(rayA, 0.02f, out RaycastHit hitA, distance + addedDistance, mask, QueryTriggerInteraction.Ignore);
            bool hittingB = Physics.SphereCast(rayB, 0.02f, out RaycastHit hitB, distance + addedDistance, mask, QueryTriggerInteraction.Ignore);
            bool hittingC = Physics.SphereCast(rayC, 0.02f, out RaycastHit hitC, distance + addedDistance, mask, QueryTriggerInteraction.Ignore);
            bool hittingD = Physics.SphereCast(rayD, 0.02f, out RaycastHit hitD, distance + addedDistance, mask, QueryTriggerInteraction.Ignore);

            // more complexe ground checking
            if((!hittingA && !hittingD) || (!hittingC && !hittingB))
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

            if(debug)
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
    }
}