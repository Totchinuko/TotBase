using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotBase.SM
{
    public abstract class StateMachineMB<M, S> : MonoBehaviour where M : StateMachineMB<M, S> where S : StateMB<M, S>
    {
        private S lastState;
        private S currentState;

        public S LastState => lastState;
        public S CurrentState => currentState;

        public void SetState(S state)
        {
            currentState?.ExitState((M)this);
            lastState = currentState;
            currentState = state;
            currentState?.EnterState((M)this);
        }

        private void Update()
        {
            currentState?.DoUpdate((M)this);
        }

        private void LateUpdate()
        {
            currentState?.DoLateUpdate((M)this);
        }

        private void FixedUpdate()
        {
            currentState?.DoFixedUpdate((M)this);
        }
    }
}