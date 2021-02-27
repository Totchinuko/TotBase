using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotBase.SM
{
    public abstract class BaseStateMB<M> : StateMB<M, BaseStateMB<M>> where M : StateMachineMB<M,BaseStateMB<M>>
    {
    }

    public abstract class BaseState<M> : State<M, BaseState<M>> where M : StateMachine<M, BaseState<M>>
    {
    }
}