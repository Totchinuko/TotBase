using UnityEngine;
using System;

namespace TotBase
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnforceComponentTypeAttribute : PropertyAttribute
    {
        public System.Type type;

        public EnforceComponentTypeAttribute(System.Type enforcedType) {
            type = enforcedType;
        }
    }
}