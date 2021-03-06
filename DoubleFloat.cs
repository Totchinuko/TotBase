using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotBase
{
    [Serializable]
    public struct DoubleFloat
    {
        public float min;
        public float max;

        public static DoubleFloat operator *(DoubleFloat d, float f)
        {
            return new DoubleFloat()
            {
                min = d.min * f,
                max = d.max * f
            };
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DoubleFloatRangeAttribute : Attribute
    {
        public float min;
        public float max;

        public DoubleFloatRangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}