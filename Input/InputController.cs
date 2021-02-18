using UnityEngine;
using System.Collections.Generic;
using System;

namespace TotBase
{
    public class InputController : MonoBehaviour
    {
        #region Singleton
        private static InputController controller;
        public static InputController Controller
        {
            get
            {
                if (controller == null)
                    controller = FindObjectOfType<InputController>();
                return controller;
            }
        }
        #endregion

        [SerializeField]
        private InputDefinition[] defaultKeys;
        [SerializeField]
        private JoystickDefinition[] defaultSticks;
        [SerializeField]
        private AxisDefinition[] axisDefinitions;
        [SerializeField]
        private bool useKeyboardDefault;

        private Dictionary<string, InputDefinition> keyMapping;
        private Dictionary<string, JoystickDefinition> stickMapping;
        private Dictionary<string, AxisDefinition> axisMapping;

        public event EventHandler ConfigChanged;

        private bool useKeyboardMouse;
        public bool UseKeyboardMouse
        {
            get
            {
                return useKeyboardMouse;
            }
            set
            {
                useKeyboardMouse = value;
                OnConfigChanged();
            }
        }

        private void Awake()
        {
            UseKeyboardMouse = useKeyboardDefault;
            SetupDefaultInputs();
        }

        public void SetupDefaultInputs()
        {
            keyMapping = new Dictionary<string, InputDefinition>();
            foreach (InputDefinition def in defaultKeys)            
                keyMapping[def.name] = def;
            stickMapping = new Dictionary<string, JoystickDefinition>();
            foreach (JoystickDefinition def in defaultSticks)
                stickMapping[def.name] = def;

            axisMapping = new Dictionary<string, AxisDefinition>();
            foreach (AxisDefinition def in axisDefinitions)
                axisMapping[def.name] = def;
        }

        public void AddKeyInputs(InputDefinition[] definitions)
        {
            if(definitions != null)
                foreach (InputDefinition def in definitions)
                    keyMapping[def.name] = def;
        }

        public void AddStickInputs(JoystickDefinition[] definitions)
        {
            if (definitions != null)
                foreach (JoystickDefinition def in definitions)
                    stickMapping[def.name] = def;
        }

        private static void GetInputs(out InputDefinition[] inputs, out JoystickDefinition[] sticks)
        {
            List<InputDefinition> keys = new List<InputDefinition>(Controller.keyMapping.Values);
            List<JoystickDefinition> joys = new List<JoystickDefinition>(Controller.stickMapping.Values);
            inputs = keys.ToArray();
            sticks = joys.ToArray();
        }

        public static KeyCode GetPrimaryKeyCode(string keyMap)
        {
            return Controller.keyMapping[keyMap].primaryKey;
        }

        public static KeyCode GetAlternateKeyCode(string keyMap)
        {
            return Controller.keyMapping[keyMap].alternatKey;
        }

        public static void SetKeyMap(string keyMap, KeyCode code, bool primary)
        {
            if (!Controller.keyMapping.ContainsKey(keyMap))
                throw new ArgumentException("Invalid KeyMap in SetKeyMap: " + keyMap);
            if(primary)
                Controller.keyMapping[keyMap].primaryKey = code;
            else
                Controller.keyMapping[keyMap].alternatKey = code;

            Controller.OnConfigChanged();
        }

        public static bool GetKeyDown(string keyMap)
        {
            return Controller.keyMapping[keyMap].GetKeyDown();
        }

        public static bool GetKey(string keyMap)
        {
            return Controller.keyMapping[keyMap].GetKey();
        }

        public static bool GetKeyUp(string keyMap)
        {
            return Controller.keyMapping[keyMap].GetKeyUp();
        }

        public static float GetJoystickAxis(string stickName)
        {
            return Controller.stickMapping[stickName].GetAxis();
        }

        public static float GetAxis(string axisName)
        {
            AxisDefinition def = Controller.axisMapping[axisName];
            if (Controller.UseKeyboardMouse && Controller.keyMapping.ContainsKey(def.keyNegative) && Controller.keyMapping.ContainsKey(def.keyPositive))
                return (GetKey(def.keyNegative) ? -1f : 0f) + (GetKey(def.keyPositive) ? 1f : 0f);
            else if (Controller.stickMapping.ContainsKey(def.joystickAxis))
                return GetJoystickAxis(def.joystickAxis);
            else
                return 0f;
        }

        public static void SetJoystickAxis(string axisName, int index)
        {
            if (!Controller.stickMapping.ContainsKey(axisName))
                throw new ArgumentException("Invalid AxisName in SetAxis: " + axisName);

            Controller.stickMapping[axisName].index = index;
        }

        public static void ClearKeyMap(string keyMap, bool primary)
        {
            if (!Controller.keyMapping.ContainsKey(keyMap))
                throw new ArgumentException("Invalid KeyMap in SetKeyMap: " + keyMap);
            if (primary)
                Controller.keyMapping[keyMap].primaryKey = KeyCode.None;
            else
                Controller.keyMapping[keyMap].alternatKey = KeyCode.None;
        }

        public static bool WaitForKeyAndSet(string keyMap, bool primary)
        {
            if (!Controller.keyMapping.ContainsKey(keyMap))
                throw new ArgumentException("Invalid KeyMap in SetKeyMap: " + keyMap);
            if (Input.anyKeyDown)
            {
                foreach (KeyCode code in (KeyCode[])Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(code))
                    {
                        if (code == KeyCode.Escape)
                            return true;
                        SetKeyMap(keyMap, code, primary);
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool WaitForStickAndSet(string axisName)
        {
            if (!Controller.stickMapping.ContainsKey(axisName))
                throw new ArgumentException("Invalid AxisName in SetAxis: " + axisName);

            int axis = WaitForAxis();
            if(axis != 0)
            {
                if (Input.GetKeyDown(KeyCode.Escape))                                    
                    return true;
                    
                SetJoystickAxis(axisName, axis);
                return true;
            }

            return false;
        }

        public static int WaitForAxis()
        {            
            for(int i = 1; i < 29; i++)
            {
                float axis = Input.GetAxisRaw($"Joy{i.ToString("00")}");
                if (axis > 0.6f)
                    return i;
            }

            return 0;
        }

        protected virtual void OnConfigChanged()
        {
            ConfigChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    [Serializable]
    public class InputDefinition
    {
        public string name;        
        public KeyCode primaryKey;
        public KeyCode alternatKey;

        public bool GetKeyDown()
        {
            return Input.GetKeyDown(primaryKey) || Input.GetKeyDown(alternatKey);
        }

        public bool GetKey()
        {
            return Input.GetKey(primaryKey) || Input.GetKey(alternatKey);
        }

        public bool GetKeyUp()
        {
            return Input.GetKeyUp(primaryKey) || Input.GetKeyUp(alternatKey);
        }
    }
    
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

    [Serializable]
    public class AxisDefinition
    {
        public string name;
        public string keyNegative;
        public string keyPositive;
        public string joystickAxis;
    }
}
