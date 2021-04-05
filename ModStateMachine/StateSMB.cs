using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TotBase.SM
{
    public abstract class StateSMB<M, S> : ScriptableObject where M : StateMachineSMB<M, S> where S : StateSMB<M, S>
    {
        public virtual void EnterState(M machine) { }
        public virtual void ExitState(M machine) { }
    }
}

