using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotBase.SM
{
    public abstract class StateMachine<M, S> : ScriptableObject where M : StateMachine<M, S> where S : State<M, S>
    {
        private S lastState;
        private S currentState;

        public S LastState => lastState;
        public S CurrentState => currentState;

        public void SetState(S state)
        {
            currentState.DoExitState((M)this);
            lastState = currentState;
            currentState = state;
            currentState.DoEnterState((M)this);
        }
    }
}
