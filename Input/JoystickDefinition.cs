using System;
using UnityEngine;

namespace TotBase
{
    [Serializable]
    public class JoystickDefinition
    {
        public string name;
        [Range(1, 28)]
        public int index;

        public float GetAxis()
        {
            return Input.GetAxisRaw($"Joy{index.ToString("00")}");
        }
    }
}