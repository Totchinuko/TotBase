using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotBase
{
    public interface IStateMachine
    {
        Enum CurrentState { get; set; }
        Enum LastState { get; }
    }
}
