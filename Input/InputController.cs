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

        private Dictionary<int, InputDefinition> keyMapping;
        private Dictionary<int, JoystickDefinition> stickMapping;
        private Dictionary<int, AxisDefinition> axisMapping;

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
            if(controller != null && controller != this)
            {
                Destroy(gameObject);
                return;
            }
            controller = this;
            DontDestroyOnLoad(gameObject);

            UseKeyboardMouse = useKeyboardDefault;
            SetupDefaultInputs();
        }

        public void SetupDefaultInputs()
        {
            keyMapping = new Dictionary<int, InputDefinition>();
            foreach (InputDefinition def in defaultKeys)            
                keyMapping[GetHash(def.name)] = def;
            stickMapping = new Dictionary<int, JoystickDefinition>();
            foreach (JoystickDefinition def in defaultSticks)
                stickMapping[GetHash(def.name)] = def;

            axisMapping = new Dictionary<int, AxisDefinition>();
            foreach (AxisDefinition def in axisDefinitions)
            {
                axisMapping[GetHash(def.name)] = def;
                def.RefreshHash();
            }                
        }

        public void AddKeyInputs(InputDefinition[] definitions)
        {
            if(definitions != null)
                foreach (InputDefinition def in definitions)
                    keyMapping[GetHash(def.name)] = def;
        }

        public void AddStickInputs(JoystickDefinition[] definitions)
        {
            if (definitions != null)
                foreach (JoystickDefinition def in definitions)
                    stickMapping[GetHash(def.name)] = def;
        }

        private static void GetInputs(out InputDefinition[] inputs, out JoystickDefinition[] sticks)
        {
            List<InputDefinition> keys = new List<InputDefinition>(Controller.keyMapping.Values);
            List<JoystickDefinition> joys = new List<JoystickDefinition>(Controller.stickMapping.Values);
            inputs = keys.ToArray();
            sticks = joys.ToArray();
        }

        public static KeyCode GetPrimaryKeyCode(string keyMap) => GetPrimaryKeyCode(GetHash(keyMap));

        public static KeyCode GetPrimaryKeyCode(int keyMap)
        {
            return Controller.keyMapping[keyMap].primaryKey;
        }

        public static KeyCode GetAlternateKeyCode(string keyMap) => GetAlternateKeyCode(GetHash(keyMap));
        public static KeyCode GetAlternateKeyCode(int keyMap)
        {
            return Controller.keyMapping[keyMap].alternatKey;
        }

        public static void SetKeyMap(string keyMap, KeyCode code, bool primary) => SetKeyMap(GetHash(keyMap), code, primary);
        public static void SetKeyMap(int keyMap, KeyCode code, bool primary)
        {
            if (!Controller.keyMapping.ContainsKey(keyMap))
                throw new ArgumentException("Invalid KeyMap in SetKeyMap: " + keyMap);
            if(primary)
                Controller.keyMapping[keyMap].primaryKey = code;
            else
                Controller.keyMapping[keyMap].alternatKey = code;

            Controller.OnConfigChanged();
        }

        public static bool GetKeyDown(string keyMap) => GetKeyDown(GetHash(keyMap));
        public static bool GetKeyDown(int keyMap)
        {
            return Controller.keyMapping[keyMap].GetKeyDown();
        }

        public static bool GetKey(string keyMap) => GetKey(GetHash(keyMap));
        public static bool GetKey(int keyMap)
        {
            return Controller.keyMapping[keyMap].GetKey();
        }

        public static bool GetKeyUp(string keyMap) => GetKeyUp(GetHash(keyMap));
        public static bool GetKeyUp(int keyMap)
        {
            return Controller.keyMapping[keyMap].GetKeyUp();
        }

        public static float GetJoystickAxis(string stickName) => GetJoystickAxis(GetHash(stickName));
        public static float GetJoystickAxis(int stickName)
        {
            return Controller.stickMapping[stickName].GetAxis();
        }

        public static float GetAxis(string axisName) => GetAxis(GetHash(axisName));
        public static float GetAxis(int axis)
        {
            AxisDefinition def = Controller.axisMapping[axis];
            if (Controller.UseKeyboardMouse && Controller.keyMapping.ContainsKey(def.keyNegativeHash) && Controller.keyMapping.ContainsKey(def.keyPositiveHash))
                return (GetKey(def.keyNegative) ? -1f : 0f) + (GetKey(def.keyPositive) ? 1f : 0f);
            else if (Controller.stickMapping.ContainsKey(def.joystickAxisHash))
                return GetJoystickAxis(def.joystickAxisHash);
            else
                return 0f;
        }

        public static bool GetAxisDown(string axisName) => GetAxisDown(GetHash(axisName));
        public static bool GetAxisDown(int axis)
        {
            AxisDefinition def = Controller.axisMapping[axis];
            if (Controller.UseKeyboardMouse && Controller.keyMapping.ContainsKey(def.keyNegativeHash) && Controller.keyMapping.ContainsKey(def.keyPositiveHash))
                return GetKey(def.keyNegative) || GetKey(def.keyPositive);
            else if (Controller.stickMapping.ContainsKey(def.joystickAxisHash))
                return !Mathf.Approximately(GetJoystickAxis(def.joystickAxisHash), 0f);
            else
                return false;
        }

        public static void SetJoystickAxis(string axisName, int index) => SetJoystickAxis(GetHash(axisName), index);
        public static void SetJoystickAxis(int axis, int index)
        {
            if (!Controller.stickMapping.ContainsKey(axis))
                throw new ArgumentException("Invalid AxisName in SetAxis: " + axis);

            Controller.stickMapping[axis].index = index;
        }

        public static void ClearKeyMap(string keyMap, bool primary) => ClearKeyMap(GetHash(keyMap), primary);
        public static void ClearKeyMap(int keyMap, bool primary)
        {
            if (!Controller.keyMapping.ContainsKey(keyMap))
                throw new ArgumentException("Invalid KeyMap in SetKeyMap: " + keyMap);
            if (primary)
                Controller.keyMapping[keyMap].primaryKey = KeyCode.None;
            else
                Controller.keyMapping[keyMap].alternatKey = KeyCode.None;
        }

        public static bool WaitForKeyAndSet(string keyMap, bool primary) => WaitForKeyAndSet(GetHash(keyMap), primary);
        public static bool WaitForKeyAndSet(int keyMap, bool primary)
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

        public static bool WaitForStickAndSet(string axisName) => WaitForStickAndSet(GetHash(axisName));
        public static bool WaitForStickAndSet(int axishash)
        {
            if (!Controller.stickMapping.ContainsKey(axishash))
                throw new ArgumentException("Invalid AxisName in SetAxis: " + axishash);

            int axis = WaitForAxis();
            if(axis != 0)
            {
                if (Input.GetKeyDown(KeyCode.Escape))                                    
                    return true;
                    
                SetJoystickAxis(axishash, axis);
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

        public static int GetHash(string name)
        {
            return Animator.StringToHash(name);
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
