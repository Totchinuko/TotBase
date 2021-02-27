using UnityEngine;
using System;

namespace TotBase
{
    [Serializable]
    public class AxisDefinition
    {
        public string name;
        public string keyNegative;
        public string keyPositive;
        public string joystickAxis;

        [HideInInspector]
        public int keyNegativeHash;
        [HideInInspector]
        public int keyPositiveHash;
        [HideInInspector]
        public int joystickAxisHash;

        public void RefreshHash()
        {
            keyNegativeHash = InputController.GetHash(keyNegative);
            keyPositiveHash = InputController.GetHash(keyPositive);
            joystickAxisHash = InputController.GetHash(joystickAxis);
        }
    }
}
