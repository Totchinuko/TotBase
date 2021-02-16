using System;
using UnityEngine;

namespace TotBase
{
    public abstract class StateMachineController<T> : StateMachine where T : StateMachineController<T>
    {
        private static T _instance;
        private static bool _stopped = false;

        public static T instance
        {
            get
            {
                if (_stopped && Application.isPlaying)
                    return null;

                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                    if (_instance != null) _instance.OnControllerAwake();
                }

                return _instance;
            }
        }

        public static bool exists
        {
            get
            {
                return instance != null;
            }
        }
        
        public static bool stopped
        {
            get
            {
                return _stopped;
            }
        }

        public static void Exec(Action<T> action)
        {
            if (exists && action != null) action.Invoke(instance);
        }

        protected virtual void Awake()
        {
            if (instance != (T)this)
                Debug.Log("[Controllers] More than one instance of " + typeof(T) + " are present.");
            else if (Application.isPlaying)
                DontDestroyOnLoad(transform.root);
        }

        protected virtual void OnApplicationQuit()
        {
            _stopped = true;
        }

        protected virtual void OnControllerAwake()
        {
        }
    }
}
