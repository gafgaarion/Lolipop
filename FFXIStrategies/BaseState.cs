using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIStrategies
{
    public abstract class BaseState : IComparable<BaseState>
    {
        public bool Enabled;
        public int Priority;
        public abstract bool CheckState();
        public abstract void EnterState();
        public abstract void RunState();
        public abstract void ExitState();

        public int CompareTo(BaseState other)
        {
            return -this.Priority.CompareTo(other.Priority);
        }
    }
}
