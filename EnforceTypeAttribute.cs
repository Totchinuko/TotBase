using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotBase
{
    public class EnforceTypeAttribute : PropertyAttribute
    {
        public System.Type type;

        public EnforceTypeAttribute(System.Type enforcedType)
        {
            type = enforcedType;
        }
    }
}
