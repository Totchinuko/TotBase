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
        public Action DoUpdate;
        public Action DoFixedUpdate;
        public Action DoLateUpdate;
        public Action DoEnterState;
        public Action DoExitState;

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

            DoUpdate = ConfigureDelegate<Action>(Update);
            DoFixedUpdate = ConfigureDelegate<Action>(FixedUpdate);
            DoLateUpdate = ConfigureDelegate<Action>(LateUpdate);
            DoEnterState = ConfigureDelegate<Action>(EnterState);
            DoEnterState = ConfigureDelegate<Action>(ExitState);

            EnterState();
        }

        protected T ConfigureDelegate<T>(T methodRoot) where T : class
        {
            return Extension.ConfigureDelegate(this, methodRoot, cache);
        }

        protected virtual void Update() { DoUpdate?.Invoke(); }
        protected virtual void FixedUpdate() { DoFixedUpdate?.Invoke(); }
        protected virtual void LateUpdate() { DoLateUpdate?.Invoke(); }
        protected virtual void ExitState() { DoExitState?.Invoke(); }
        protected virtual void EnterState() { DoEnterState?.Invoke(); }
    }
}
