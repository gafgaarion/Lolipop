using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIMemoryObserver.Exceptions
{
    public class AllInstanceAlreadyBoundException : Exception
    {

        public AllInstanceAlreadyBoundException(string message = "All FFXI instances are already bound to server.")
            : base(message)
        { }
    }
}
