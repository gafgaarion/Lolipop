using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIMemoryObserver
{
    public interface IMemoryInspector
    {
        void Start();
        void Terminate();
        string getLeaderId();
        void setLeaderId(string id);
    }
}
