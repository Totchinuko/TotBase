using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TotBase
{    
    public class StateMachine : MonoBehaviour, IStateMachine
    {
        #region fields
        public Action DoUpdate = DoNothing;
        public Action DoFixedUpdate = DoNothing;
        public Action DoLateUpdate = DoNothing;
        public Action EnterState = DoNothing;
        public Action ExitState = DoNothing;

        private Enum lastState;
        private Enum currentState;
        private float timeStateBegin;
        private Dictionary<Enum, Dictionary<string, Delegate>> cache = new Dictionary<Enum, Dictionary<string, Delegate>>();        
        #endregion

        #region Properties
        public Enum CurrentState
        {
            get { return currentState; }
            set
            {
                if (value.Equals(currentState))
                    return;

                lastState = currentState;
                timeStateBegin = Time.time;
                currentState = value;
                ConfigureCurrentState();
            }
        }

        public Enum LastState
        {
            get
            {
                return lastState;
            }
        }

        public float TimeStateBegin
        {
            get
            {
                return timeStateBegin;
            }
        }
        #endregion

        /// <summary>
        /// Update default delegates with the current state.
        /// </summary>
        protected virtual void ConfigureCurrentState()
        {
            OnExitState();

            DoUpdate = ConfigureDelegate<Action>("Update", DoNothing);
            DoFixedUpdate = ConfigureDelegate<Action>("FixedUpdate", DoNothing);
            DoLateUpdate = ConfigureDelegate<Action>("LateUpdate", DoNothing);
            EnterState = ConfigureDelegate<Action>("EnterState", DoNothing);
            ExitState = ConfigureDelegate<Action>("ExitState", DoNothing);

            OnEnterState();
        }

        protected T ConfigureDelegate<T>(string methodRoot, T defaultMethod) where T : class
        {
            return Extension.ConfigureDelegate(this, methodRoot, defaultMethod, cache);
        }

        protected static void DoNothing() { }
        protected static void DoNothing(Vector3 a) { }
        protected static void DoNothing(Quaternion a) { }
        protected static void DoNothing(float a) { }
        protected static void DoNothing(bool a) { }

        protected virtual void Update() { if(DoUpdate != null) DoUpdate.Invoke(); }
        protected virtual void FixedUpdate() { if (DoFixedUpdate != null) DoFixedUpdate.Invoke(); }
        protected virtual void LateUpdate() { if (DoLateUpdate != null) DoLateUpdate.Invoke(); }
        protected virtual void OnExitState() { if (ExitState != null) ExitState.Invoke(); }
        protected virtual void OnEnterState() { if (EnterState != null) EnterState.Invoke(); }
    }
}
