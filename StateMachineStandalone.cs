using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotBase
{
    public class StateMachineStandalone : IStateMachine
    {
        #region fields
        public Action DoEnterState = DoNothing;
        public Action DoExitState = DoNothing;

        private Enum currentState;
        private Enum lastState;
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
            ExitState();

            DoEnterState = ConfigureDelegate<Action>(EnterState, DoNothing);
            DoExitState = ConfigureDelegate<Action>(ExitState, DoNothing);

            EnterState();
        }

        protected T ConfigureDelegate<T>(T methodRoot, T defaultMethod) where T : class
        {
            return Extension.ConfigureDelegate(this, methodRoot, defaultMethod, cache);
        }

        protected virtual void EnterState() { DoEnterState?.Invoke(); }
        protected virtual void ExitState() { DoExitState?.Invoke(); }

        protected static void DoNothing() { }
        protected static void DoNothing(float a) { }
        protected static void DoNothing(bool a) { }
    }
}
