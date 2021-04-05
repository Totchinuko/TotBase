using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotBase.SM
{
    public abstract class StateMachineSMB<M, S> : MonoBehaviour where M : StateMachineSMB<M, S> where S : StateSMB<M, S>
    {
        private S lastState;
        private S currentState;

        public S LastState => lastState;
        public S CurrentState => currentState;

        public float LastEnterState { get; private set; }

        public virtual void SetState(S state)
        {
            currentState?.ExitState((M)this);
            lastState = currentState;
            currentState = state;
            currentState?.EnterState((M)this);
            LastEnterState = Time.time;
        }
    }
}