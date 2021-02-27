using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TotBase.SM
{
    public abstract class StateMB<M, S> : ScriptableObject where M : StateMachineMB<M, S> where S : StateMB<M, S>
    {
        public virtual void EnterState(M machine) { }
        public virtual void ExitState(M machine) { }
        public virtual void DoUpdate(M machine) { }
        public virtual void DoLateUpdate(M machine) { }
        public virtual void DoFixedUpdate(M machine) { }
    }
}

