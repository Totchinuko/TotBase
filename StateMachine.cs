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
        public Action DoEnterState = DoNothing;
        public Action DoExitState = DoNothing;

        private Enum lastState;
        private Enum currentState;
        private float timeStateBegin;
        private Dictionary<Enum, Dictionary<int, Delegate>> cache = new Dictionary<Enum, Dictionary<int, Delegate>>();        
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
            ExitState();

            DoUpdate = ConfigureDelegate<Action>(Update, DoNothing);
            DoFixedUpdate = ConfigureDelegate<Action>(FixedUpdate, DoNothing);
            DoLateUpdate = ConfigureDelegate<Action>(LateUpdate, DoNothing);
            DoEnterState = ConfigureDelegate<Action>(EnterState, DoNothing);
            DoEnterState = ConfigureDelegate<Action>(ExitState, DoNothing);

            EnterState();
        }

        protected T ConfigureDelegate<T>(T methodRoot, T defaultMethod) where T : class
        {
            return Extension.ConfigureDelegate(this, methodRoot, defaultMethod, cache);
        }

        protected static void DoNothing() { }
        protected static void DoNothing(Vector3 a) { }
        protected static void DoNothing(Quaternion a) { }
        protected static void DoNothing(float a) { }
        protected static void DoNothing(bool a) { }

        protected virtual void Update() { DoUpdate?.Invoke(); }
        protected virtual void FixedUpdate() { DoFixedUpdate?.Invoke(); }
        protected virtual void LateUpdate() { DoLateUpdate?.Invoke(); }
        protected virtual void ExitState() { DoExitState?.Invoke(); }
        protected virtual void EnterState() { DoEnterState?.Invoke(); }
    }
}
