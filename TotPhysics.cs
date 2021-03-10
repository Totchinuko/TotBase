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
        public static bool GetFourPointGroundData(Vector3 origin, Vector3 forward, float radius, float distance, LayerMask mask, out GroundData data)
        {
            // testing if we are on the ground first            
            RaycastHit hit;
            if(!Physics.SphereCast(origin, radius, -Vector3.up, out hit, distance, mask, QueryTriggerInteraction.Ignore))
            {
                data = new GroundData()
                {
                    position = hit.point,
                    normal = Vector3.up,
                    angle = 0f
                };
                return false;
            }

            Vector3 right = Vector3.Cross(Vector3.up, forward);
            
            Ray rayA = new Ray(origin + (forward + right).normalized * radius, Vector3.down);
            Ray rayB = new Ray(origin + (forward + -right).normalized * radius, Vector3.down);
            Ray rayC = new Ray(origin + (-forward + right).normalized * radius, Vector3.down);
            Ray rayD = new Ray(origin + (-forward + -right).normalized * radius, Vector3.down);

            bool hittingA = Physics.SphereCast(rayA, 0.02f, out RaycastHit hitA, distance, mask, QueryTriggerInteraction.Ignore);
            bool hittingB = Physics.SphereCast(rayA, 0.02f, out RaycastHit hitB, distance, mask, QueryTriggerInteraction.Ignore);
            bool hittingC = Physics.SphereCast(rayA, 0.02f, out RaycastHit hitC, distance, mask, QueryTriggerInteraction.Ignore);
            bool hittingD = Physics.SphereCast(rayA, 0.02f, out RaycastHit hitD, distance, mask, QueryTriggerInteraction.Ignore);

            // more complexe ground checking
            if((!hittingA && !hittingD) || (!hittingC && !hittingB))
            {
                data = new GroundData()
                {
                    position = hit.point,
                    normal = Vector3.up,
                    angle = 0f
                };
                return false;
            }

            Vector3 pointA = hittingA ? hitA.point : hitD.point + (hit.point - hitD.point) * 2;
            Vector3 pointB = hittingB ? hitB.point : hitC.point + (hit.point - hitC.point) * 2;
            Vector3 pointC = hittingC ? hitC.point : hitB.point + (hit.point - hitB.point) * 2;
            Vector3 pointD = hittingD ? hitD.point : hitA.point + (hit.point - hitA.point) * 2;

            Vector3 normal = Vector3.Cross(pointA - pointD, pointC - pointB).normalized;
            data = new GroundData()
            {
                position = hit.point,
                normal = normal,
                angle = Mathf.Round(Vector3.Angle(normal, Vector3.up))
            };
            return true;
        }  
    }
}