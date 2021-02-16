using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotBase
{
    public class StateMachineStandalone : IStateMachine
    {
        #region fields
        public Action EnterState = DoNothing;
        public Action ExitState = DoNothing;

        private Enum currentState;
        private Enum lastState;
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
                currentState = value;
                ConfigureCurrentState();
            }
        }

        public Enum LastState
        {
            get { return lastState; }
        }
        #endregion

        /// <summary>
        /// Update default delegates with the current state.
        /// </summary>
        protected virtual void ConfigureCurrentState()
        {
            if (ExitState != null) ExitState.Invoke();

            EnterState = ConfigureDelegate<Action>("EnterState", DoNothing);
            ExitState = ConfigureDelegate<Action>("ExitState", DoNothing);

            if (EnterState != null) EnterState.Invoke();
        }

        protected T ConfigureDelegate<T>(string methodRoot, T defaultMethod) where T : class
        {
            return Extension.ConfigureDelegate(this, methodRoot, defaultMethod, cache);
        }

        protected static void DoNothing() { }
        protected static void DoNothing(float a) { }
        protected static void DoNothing(bool a) { }
    }
}
