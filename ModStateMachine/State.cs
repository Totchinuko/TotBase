using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotBase.SM
{
    public abstract class State<M, S> : ScriptableObject where M : StateMachine<M, S> where S : State<M, S>
    {
        public virtual void DoEnterState(M machine) { }
        public virtual void DoExitState(M machine) { }
    }
}
