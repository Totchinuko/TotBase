using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotBase
{
    [CreateAssetMenu(fileName = "InputControllerConfig", menuName = "TotBase/InputControllerConfig")]
    public class InputControllerConfig : ScriptableObject
    {
        public InputDefinition[] keys;
        public JoystickDefinition[] sticks;
    }
}

